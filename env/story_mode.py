import time
import pygetwindow as gw
import pyautogui


def find_gta_window():
    """
    Returns fixed bounds for the GTA V window, assuming it is centered at 1280x720 on a 1920x1080 screen.
    """
    screen_width, screen_height = pyautogui.size()

    window_width = 1280
    window_height = 720

    offset_x = (screen_width - window_width) // 2
    offset_y = (screen_height - window_height) // 2

    left = offset_x
    top = offset_y
    right = left + window_width
    bottom = top + window_height

    return left, top, right, bottom


def click_bottom_right(window_bounds):
    """
    Clicks the bottom-right corner of the specified window.
    """
    left, top, right, bottom = window_bounds
    click_x = right - 50
    click_y = bottom - 100
    print(f"Clicking at ({click_x}, {click_y})")
    pyautogui.click(click_x, click_y)

def wait_for_story_mode_screen():
    """
    Waits for the GTA V main menu screen to load and clicks the bottom-right corner.
    """
    print("Waiting for GTA V main menu to load...")
    while True:
        window_bounds = find_gta_window()
        if window_bounds:
            print(f"GTA V window detected at bounds: {window_bounds}")
            
            time.sleep(70)
            
            click_bottom_right(window_bounds)
            break
        else:
            print("GTA V window not detected. Retrying...")
        time.sleep(1)

def choose_storymode_if_possible():
    """
    Checks if the GTA V window is active and clicks the bottom-right corner when detected.
    """
    while True:
        gta_window = find_gta_window()
        if gta_window:
            print(f"GTA V window detected: {gta_window}")
            wait_for_story_mode_screen()
            break
        else:
            print("GTA V window not found. Retrying...")
        time.sleep(5)
