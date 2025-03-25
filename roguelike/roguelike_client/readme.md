+-------------------+       +-------------------+       +-------------------+
|    Game Scene     |<----->| ClientStateManager|<----->| NetworkManagement |
| (Render, Input)   |       | (State, Prediction)|      | (UDP, Signals)    |
+-------------------+       +-------------------+       +-------------------+
                                    |                        |
                                    |                        |
                                    v                        v
                            +----------------+       +----------------+
                            | LocalUser, Room|       | NetworkClient  |
                            | Input History  |       | (Send/Receive) |
                            +----------------+       +----------------+
                                                            |
                                                            v
                                                    +----------------+
                                                    | Rust UDP Server |
                                                    +----------------+
