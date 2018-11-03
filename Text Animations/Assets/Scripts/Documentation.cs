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
    
    Tasks
        - New class "Letter" to access the GameObject "Letter"                                               DONE
        - New animation: "Heart Beat"                                                                        DONE
        - New Animation: "Heart Beat Without Alpha"                                                          DONE
        - Pause time between instance of animations                                                          DONE
        - Include Text component into WordAnimator component and extract properties from there
            - Replace Word property for text.text property                                                   DONE
            - Replace fontSize property for text.fontSize property                                           DONE
            - Replace WordAnimator.position for WordAnimator.rectTransform.localPosition                     DONE


 */ 
