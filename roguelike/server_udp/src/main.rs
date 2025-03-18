use std::{ error::Error};

use clap::Parser;
use config::globals;
use network::message;
use tokio::runtime::Builder;

pub mod config;
pub mod game;
pub mod network;
pub mod server;
pub mod utils;

#[derive(Parser, Debug)]
#[command(about = "UDP server for roguelike game")]
struct Args {
    #[arg(
        short,
        long,
        require_equals = true,
        default_value_t = globals::DEFAULT_PORT,
        help = "PORT NUMBER used for server init")]
    port: u16,

    #[arg(short, long, help = "Enable tracing of UDP messages on console log.")]
    trace: bool,
}

// Run server: cargo run -- --port=8082 --trace
fn main() -> Result<(), Box<dyn Error>> {
    let args = Args::parse();

    if args.trace {
        message::set_trace(true);
        println!("Message tracing enabled");
    }

    // Create tokio threadpool with 6 threads
    match Builder::new_multi_thread()
        .worker_threads(6)
        .enable_all()
        .build()
    {
        Ok(run_time) => {
            println!("Tokio runtime successfully created");
            run_time.block_on(async {
                // Start server here
                match server::start_server(args.port).await {
                    Ok(_) => {
                        println!("Server started successfully on port {}. Waiting for CTRL + C to shutdown", args.port);

                        match tokio::signal::ctrl_c().await {
                            Ok(_)=> println!("\n CTRL + C interrupt received. Shutting down server gracefully..."),

                            Err(e) => eprintln!("Failed to listen for CTRL + C event: {}", e),
                        }
                    }
                    Err(e) => {
                        eprintln!("Server failed to start: {}", e);
                        std::process::exit(1);
                    }
                }
            })
        }
        Err(err) => {
            eprintln!("Something went wrong: {}", err);
            std::process::exit(1);
        }
    };

    Ok(())
}
