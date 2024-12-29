# AutoGuard
Implementation of autonomous self driving test environment using GTA V for future autonomous driving security and functional safety tests.

I am currently using the [seamless autonomous driving mod](https://www.gta5-mods.com/scripts/seamless-autonomous-driving-mod-no-keys-menus-or-buttons) to test autonomous driving in GTA V. However, for effective testing, it is essential to have a component that can interact with the graphical interface of GTA V, providing inputs and monitoring outputs dynamically.

## Getting Started

### Requirements:
 - python 3.13
 - `pip install -r requirements.txt`

### Prerequisites
1. **GTA V** installed and 1280x720 screen resolution for the game
2. **.NET Framework** for running the C# mod
3. Dependencies:
   - Rockstar Games Launcher (and be already logged in)

### Running
run `python env/app.py`

### How it works
- `env` - python script to load mods, saves and run game
- `mod` - C# gtav mod to set target points on map, find the nearest car to drive and start driving to enable `seamless autonomous driving mod`  
https://www.gta5-mods.com/scripts/seamless-autonomous-driving-mod-no-keys-menus-or-buttons
- `processing` - python scripts to analyzing data from test videos

### Basic flow  
```mermaid
sequenceDiagram
    participant user as user
    participant script as script
    participant GTA as GTA V
    participant recorder as screen recorder
    participant file as data file
    participant db as db

    user->>script: run script
    script->>GTA: Launch GTA V
    script-->>script: Wait for GTA V to load
    script->>GTA: Select game mode
    script->>GTA: Start car driving
    script->>recorder: Start screen recording
    par During car driving
        GTA-->>script: Provide data
        script->>file: Append data to file
        recorder-->>script: Send live recording
    end
    script->>script: Process results (data + recording)
    script->>user: Display results
    script->>db: Save results
```

### TODO
1. process somehow video from game and get data from it (eventually C# script to get sensors data)
2. add db (probably mongodb)
3. be prepared to integrate with other autonomous riding envs (other than seamless autonomous driving mod)

#### shortterm
- save data from testcases to separate files

### More informations
[more docs](https://docs.google.com/document/d/1IKcRw_cjcgbgFVxM3nnlapJooMkW_Ll9Ibul6B54esw)
