import cv2
import numpy as np
from mss import mss
import pygetwindow as gw

class GTACapture:
    def __init__(self, window_title="Grand Theft Auto V"):
        self.window_title = window_title
        self.monitor = self.get_window_dimensions()
        self.sct = mss()

    def get_window_dimensions(self):
        """Find the window position and dimensions based on the title."""
        try:
            gta_window = gw.getWindowsWithTitle(self.window_title)[0]
            return {
                "top": gta_window.top,
                "left": gta_window.left,
                "width": gta_window.width,
                "height": gta_window.height
            }
        except IndexError:
            raise ValueError(f"Window with title '{self.window_title}' not found. Ensure GTA V is running in windowed mode.")

    def capture_frame(self):
        """Capture a single frame from the specified monitor."""
        frame = np.array(self.sct.grab(self.monitor))
        return cv2.cvtColor(frame, cv2.COLOR_BGRA2BGR)

class ImageSaver:
    def __init__(self, output_folder="./images"):
        self.output_folder = output_folder
        self.frame_count = 0

    def save_frame(self, frame):
        """Save the given frame as an image."""
        filename = f"{self.output_folder}/frame_{self.frame_count}.png"
        cv2.imwrite(filename, frame)
        self.frame_count += 1

class VideoSaver:
    def __init__(self, output_file="./capture.avi", fps=30, resolution=(1280, 720)):
        self.output_file = output_file
        self.fourcc = cv2.VideoWriter_fourcc(*'XVID')
        self.out = cv2.VideoWriter(output_file, self.fourcc, fps, resolution)

    def save_frame(self, frame):
        """Save the given frame to the video file."""
        self.out.write(frame)

    def release(self):
        """Release the video writer."""
        self.out.release()


if __name__ == "__main__":
    gta_capture = GTACapture()
    resolution = (gta_capture.monitor["width"], gta_capture.monitor["height"])
    video_saver = VideoSaver(output_file="./capture.avi", resolution=resolution)

    while True:
        frame = gta_capture.capture_frame()
        video_saver.save_frame(frame)

        cv2.imshow("GTA V - Capture", frame)

        if cv2.waitKey(1) & 0xFF == ord("q"):
            break

    video_saver.release()
    cv2.destroyAllWindows()
