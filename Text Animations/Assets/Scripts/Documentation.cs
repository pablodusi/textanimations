/*

    Errors:

            - When the animation is set in "not loop" the animation stops on a wrong frame.                  DONE
            - Actually, you can't set a word with more than one row                                          DONE
            - The position behaves weird when you change the font size repeteadly in the animation 2 and 4.  DONE
            - If you change the font size repeteadly it is not reflected on the animations                   DONE
            - When you change the Word property, nothing happens. The word have to be reconstructed.         DONE
            - The GameObject letters always are in the same position. It has to be configurable.             DONE
            - SetInvisible boolean property when the animation ends. If false, it shows the first frame      DONE
                                                                     If true, it shows nothing.
            - Currently, there's no more than one font.
            - When you change the position while the animation is playing, nothing happens.                  DONE
            - When you change the font Size if there's a line break, it's not respected.                     DONE
            - When you change the position, if there's a line break, it's not respected                      DONE
            - When you pass from "Shake" animation to other animation, the letters are misplaced             DONE
            - There are animations that have to use textColor property                                       DONE
            - When heart beat animation plays, the first or last frame don't uses the real alpha             DONE
                NOTE: Search for SetLettersVisible
            - Some alpha values doesn't change when you change the color property                            DONE
            - Solve warning "no script for GameObjectSelectionItem"
            - When a letter changes its font size then the size of the rectTransform must change proportionaly
            - Limit public properties values min max
    
    Tasks
        - New class "Letter" to access the GameObject "Letter"                                               DONE
        - New animation: "Heart Beat"                                                                        DONE
        - New Animation: "Heart Beat Without Alpha"                                                          DONE
        - Pause time between instance of animations                                                          DONE
        - Include Text component into WordAnimator component and extract properties from there               DONE
            - Replace Word property for text.text property                                                   DONE
            - Replace fontSize property for text.fontSize property                                           DONE
            - Replace WordAnimator.position for WordAnimator.rectTransform.localPosition                     DONE
            - Replaced WordAnimator.textColor to WordAnimator.text.color                                     DONE
        - Constantly update Text and Rect Transforms components constantly                                   DONE
        - New Animation: "Automatic Writing"                                                                 DONE
        - Fuse animations that make the same but uses the alpha in inverse way				                 DONE
        - Ability to combine more than one animation at the time
        - New Animation: "Auotamic Writing" with "Animation 1"						                         DONE
        - New Animation: "Fireworks 1"                                                                       DONE
        - New Animation: "Fireworks 2"                                                                       DONE
        - New Animation: "Fireworks 3"                                                                       DONE
        - New Animation: "Fireworks 4": It shots all trails at the same time                                 DONE
        - New Animation: "Swinging 1": It swings all letters from top to bottom and viceversa                DONE
        - New Animation: "Swinging 2": It swings all letters from left to right and viceversa		         DONE
        - New Animation: "Rotation 1": It rotates from one Z point to another Z point (360 circuit)          DONE
        - New Animation: "Rotation 2": It rotates from one Z point to another Z point and viceversa
        - New Animation: "Rotation 3": It rotates from one Y point to another Y point and viceversa
        - New Boolean, fontSizeVariations: If it's activated, then the fontSize in the animation will be a random between two values
        - New Animation: "Swinging 3": It swings all pair letters different from odd letters from top to bottom
        - New Animation: "Font Size 1" One letter by one, it changes font size from min to max, from 1 alpha to 0 alpha,
        when the letter animation stops, the letter dissapears, one by one to the last letter
        - New Animation: "Random Color 1": Every frame, it changes the color of the text, randonmly.
        - New Animation: "Random Color 2": Every frame, it changes the color of the every letter, separately, randomly
        - New Animation: "Lerp Color 1": It interpolates from one color to another, and viceversa-
        - New Animation: "Lerp Color 2": It interpolates from one color to another, using the alpha from 1 to 0
        - New Animation: "Lerp Color 3": It interpolates from one color to another, every letter separately, with start and end random colors.
        - New Animation: "Shake 2": Shakes only on the Y axis
        - New Animation: "Shake 3": Shakes only in the X axis
        - New Animation: "Lerp Scale": Interpolates between two values on the X axis scale
        - New Animation: "Moving Poster": Every letters changes his index to the next one every "x" time
        - New Animation: "Swinging Text": Rotation X
        - Update firework animations: Start Color and End Color in Particle Systems.
*/ 
