```mermaid
sequenceDiagram
    participant User as User
    participant App as Application
    participant GTA as GTA V
    participant Recorder as Screen Recorder

    User->>App: Start application
    App->>GTA: Launch GTA V
    App-->>App: Wait for GTA V to load
    App->>GTA: Select game mode and save
    App->>GTA: Simulate F9 press
    App->>Recorder: Start screen recording
    Recorder-->>App: Send recording
    App->>App: Process results
    App->>User: Display results

```