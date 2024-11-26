import cv2
import numpy as np
from mss import mss

monitor = {"top": 100, "left": 100, "width": 1280, "height": 720}

with mss() as sct:
    while True:
        frame = np.array(sct.grab(monitor))
        frame = cv2.cvtColor(frame, cv2.COLOR_BGRA2BGR)

        cv2.imshow("GTA V", frame)

        if cv2.waitKey(1) & 0xFF == ord("q"):
            break

cv2.destroyAllWindows()
