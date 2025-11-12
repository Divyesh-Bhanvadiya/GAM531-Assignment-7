# Assignment 7 - Sprite Animation in OpenGL

This project extends the `SpriteGameOpenTk` base, implementing advanced sprite animation mechanics for a character.
It features a state-driven animation system utlizing sprite-sheet, adding following actions:

- walking
- running
- jumping
- attacking
- shielding/blocking

## Program Results
  ![](https://github.com/Divyesh-Bhanvadiya/GAM531-Assignment-7/blob/master/Program%20Results.gif)

## Features

*   OpenGL 3.3 pipeline: VAO/VBO, shaders, uniforms.
*   Orthographic 2D rendering in pixel coordinates.
*   Texture loading via ImageSharp (PNG → RGBA).
*   Sprite-sheet sampling (`uOffset`, `uSize`).
*   **State-driven animation system** for `Idle`, `Walk`, `Run`, `Jump`, `Attack1`, `Attack2`, `Attack3`, and `Shield` states.
*   **Running mechanic:** Increased movement speed and dedicated animation when `Shift` is held.
*   **Jumping mechanic:** Triggers jump animation
*   **Attack and Shield animations:** Non-looping animations that return to idle upon completion.
*   Character horizontal flipping (left-right logic) based on movement direction.

## How State Machine is implemented

### TL;DR
*   **Character Logic**: Manages player state (`Idle`, `Walk`, `Run`, `Jump`, `Attack`, `Shield`), processes keyboard input, updates character position (horizontally), and controls animation transitions.
*   **Animation Controller**: Advances frames every `FrameTime` based on the current `CharacterState`. Handles looping and non-looping animations, ensuring non-looping animations hold their last frame before transitioning back to `Idle`.
---
The character's behavior and animation are managed by a state machine primarily
implemented across the Character and AnimationController classes, driven by the CharacterState enumeration.

The Character class acts as the central state manager: it continuously evaluates player input (e.g., arrow
keys, Shift, Spacebar, attack keys) to determine state, and then attempts to transition to a newState if
possible, preventing interruptions for actions like attacks or jumps. 

Once a new state is established, the Character class instructs the AnimationController to actually animate the new state. The AnimationController handles the visual aspect,
advancing frames, managing loop behavior (e.g., looping for Walk/Run, holding the last frame for non-looping
actions like Attack or Jump), and updating shader uniforms to display the correct sprite frame. After a
non-looping animation finishes, the Character automatically transitions back to the Idle state, ensuring a
smooth return to a neutral pose.



## Requirements

*   **.NET SDK**: 6.0 or newer.
*   **OpenGL**: 3.3+ capable GPU/driver.
*   OS: Windows, macOS, or Linux.

## Controls

*   **Right Arrow**: Walk right.
*   **Left Arrow**: Walk left.
*   **Hold Shit** to run .
* **Spacebar**: Jump.
*   **A**: Trigger Attack 1.
*   **S**: Trigger Attack 2.
*   **D**: Trigger Attack 3.
*   **X (HOLD)**: Trigger Shield.
*   **Release movement keys**: Character remains in the last frame. 
*   **Release non-looping animation key (Attack, Jump, Shield)**: Animation completes and character returns to `Idle`.

## Sprite Sheet Layout (`My_Shinobi_Spritelist.png`)

*   Each frame's width: 128px
*   Each frame's height: 126px
*   Gap between frames: 0px

The rows are organized as follows:
*   **Row 0**: Walk (8 frames) - also used for Idle (first frame)
*   **Row 1**: Run (8 frames)
*   **Row 2**: Jump (12 frames)
*   **Row 3**: Attack 1 (5 frames)
*   **Row 4**: Attack 2 (3 frames)
*   **Row 5**: Attack 3 (4 frames)
*   **Row 6**: Shield/Block (4 frames)


## License
MIT 
