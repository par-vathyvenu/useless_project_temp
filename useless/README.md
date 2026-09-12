# 🐾 ശല്യക്കാരൻ പൂച്ച (The Deliberately Annoying Desktop Cat)

A native Windows desktop companion application built with C# and WPF. The cat lives directly on your Windows desktop as a lightweight, frameless, transparent overlay. It wanders around, deliberately stands in front of active windows, dances, stares judgmentally at your code, reacts when poked, disappears and reappears unpredictably, and delivers sarcastic Malayalam dialogues.

---

## ✨ Features & Behaviors

* **Transparent, Frameless Overlay**: Floats on top of all windows with true per-pixel transparency. Non-cat areas are completely click-through, ensuring it never interferes with typing or background apps unless clicked directly.
* **Natural Movement & Smooth Walk Cycle**: Synchronized 6-frame walk cycle extracted directly from your sprite sheets. Flips dynamically when turning left or right without feet-sliding or teleporting.
* **Unpredictable Behavior Machine**:
  * **Idle & Skeptical Stare**: Sits down and judges your coding or productivity.
  * **Screen Center Hijack ("Center Stage")**: Suddenly scampers right into the center of your screen, blocks your view, sits down, and refuses to leave.
  * **Dance Mode**: Stands up on two hind legs, waving paws and bobbing up and down excitedly.
  * **Playful Pounce**: Crouches in a butt-up wiggle before executing a smooth parabolic leap forward.
  * **Disappear & Reappear**: Says goodbye (*"ഞാൻ പോകുവാ..."*), fades out, vanishes for 15–40 seconds, and reappears somewhere unexpected!
* **Hilarious Sarcastic Malayalam Dialogues**:
  * Authentic quotes with English subtitles shown in a comic-style speech bubble above the cat.
  * Animated mouth frames that cycle during speech, making the cat look like it is actively talking/meowing!
  * Includes productivity shaming (*"എന്താ നോക്കുന്നേ? പണിയില്ലേ?"*, *"ഓ... വലിയ കോഡിങ്! ബഗ്ഗ് മുഴുവൻ ഇവിടെ ഉണ്ടല്ലോ!"*, *"പോയി വല്ല പണിയും എടുക്ക് ഷാജീ!"*), screen-blocking remarks (*"ഞാൻ ഇവിടെ ഇരിക്കും. നീ എന്ത് ചെയ്യും?"*), and entrance quips.
* **Interactive Reactions**:
  * **Left Click**: Interrupts the cat instantly. The cat jumps up in surprise with wide eyes, complains in Malayalam, and scampers away if poked repeatedly!
  * **Click & Drag**: Pick up and reposition the cat anywhere on the desktop. The cat flails its legs in mid-air (*"എന്നെ എങ്ങോട്ടാ വലിച്ചു കൊണ്ടുപോകുന്നേ?!"*).
  * **Right Click**: Opens a context menu directly on the cat.
* **Windows System Tray Integration**:
  * Sits quietly in the notification area with a custom cat icon.
  * Double-click tray icon to summon the cat to the center.
  * Right-click tray icon to access snooze (nap mode), speech triggers, dance mode, or clean exit.
* **Fault-Tolerant Asset Engine**:
  * Automatically scans `assets/sprites/` on startup.
  * If extra animation frames are added, they are automatically included in the animation loops.
  * If any frame is missing, it falls back to the default idle frame gracefully without crashing.
* **Zero Python & Zero Dependencies**:
  * Pure native Windows .NET/WPF application.
  * Requires no Python, no Node.js, and no external runtime installers.

---

## 🚀 How to Run

### Method 1: Double-Click Launch (Recommended)
Simply double-click **`run.bat`** in this folder!

The cat will immediately appear on your desktop above the taskbar, say hello, and begin its antics.

### Method 2: PowerShell
```powershell
powershell -STA -NoProfile -ExecutionPolicy Bypass -File .\run.ps1
```

---

## 🛠️ How to Build / Rebuild

To recompile the project at any time:
1. Double-click **`build.bat`**, or
2. Open PowerShell and run:
   ```powershell
   & "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" AnnoyingCat.csproj /p:Configuration=Release /t:Rebuild
   ```
The compiled standalone binary is placed at:
`bin\Release\AnnoyingCat.exe`

You can also open `AnnoyingCat.sln` directly in **Visual Studio** or **JetBrains Rider**.

---

## 🎮 Controls & Shortcuts

| Action | Result |
| :--- | :--- |
| **Left Click on Cat** | Jumps in surprise, complains in Malayalam. Scamper away if clicked 3 times. |
| **Click & Drag** | Pick up and move the cat around the screen while it flails its paws. |
| **Right Click on Cat** | Opens context menu (Snooze, Center, Quip, Dance, Pounce, Shoo, Exit). |
| **Double Click Tray Icon** | Summons the cat to screen center immediately. |
| **Right Click Tray Icon** | Opens tray control menu to snooze or quit. |

---

## 📁 Project Structure

```
useless/
├── assets/
│   └── sprites/
│       ├── walk/            # walk_0.png .. walk_5.png (6-frame walk cycle)
│       ├── idle/            # idle_stare.png, idle_look_up.png, idle_happy.png, etc.
│       ├── dance/           # dance_0.png, dance_1.png (paws up dancing)
│       ├── mouth/           # mouth_0.png .. mouth_5.png (talking mouth sync)
│       ├── actions/         # surprised.png (click reaction), pounce.png, playful.png
│       └── icon.ico         # App & tray icon
├── src/
│   ├── App.xaml             # Application definition
│   ├── App.xaml.cs          # Single-instance guard & lifecycle
│   ├── MainWindow.xaml      # Transparent overlay & speech bubble UI
│   ├── MainWindow.xaml.cs   # Render loop, drag/drop, hit testing, audio
│   ├── CatController.cs     # Behavior state machine (Idle, Walk, Block, Dance, etc.)
│   ├── MovementManager.cs   # Desktop pathing, speed & boundary clamping
│   ├── DialogueManager.cs   # Authentic sarcastic Malayalam dialogue collection
│   ├── AssetManager.cs      # Fault-tolerant sprite loader
│   └── TrayIconManager.cs   # System tray notification icon & menu
├── tools/
│   └── SpriteExtractor.cs   # Native C# tool that sliced & transparentized the sprites
├── AnnoyingCat.csproj       # Standard MSBuild / Visual Studio project file
├── AnnoyingCat.sln          # Visual Studio solution file
├── build.bat                # 1-click build script
├── run.bat                  # 1-click run script
├── run.ps1                  # PowerShell launcher with in-memory fallback
└── bin/Release/
    └── AnnoyingCat.exe      # Precompiled ready-to-run desktop application
```
