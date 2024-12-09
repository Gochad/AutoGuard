import time
import pyautogui
import pygetwindow as gw
import pydirectinput

def find_gta_window():
    windows = gw.getWindowsWithTitle("Grand Theft Auto V")
    if windows:
        w = windows[0]
        if w.width > 0 and w.height > 0:
            return w.left, w.top, w.right, w.bottom
    return None

def activate_gta_window():
    gta_window = gw.getWindowsWithTitle("Grand Theft Auto V")
    if gta_window:
        gta_window[0].activate()
        print("Activated GTA V window.")
    else:
        print("GTA V window not found.")

def click_story_mode(window_bounds):
    left, top, right, bottom = window_bounds
    
    click_x = right - 100
    click_y = bottom - 20

    print(f"Clicking at ({click_x}, {click_y}) for STORY MODE")

    pydirectinput.moveTo(click_x, click_y)
    time.sleep(0.5)
    pydirectinput.press('enter')

def wait_for_story_mode_screen():
    print("Waiting for GTA V main menu to load...")
    while True:
        window_bounds = find_gta_window()
        if window_bounds:
            print(f"GTA V window detected at bounds: {window_bounds}")

            activate_gta_window()

            time.sleep(70)

            click_story_mode(window_bounds)
            break
        else:
            print("GTA V window not detected. Retrying...")
        time.sleep(1)

def choose_storymode_if_possible():
    while True:
        gta_window = find_gta_window()
        if gta_window:
            print(f"GTA V window detected: {gta_window}")
            wait_for_story_mode_screen()
            break
        else:
            print("GTA V window not found. Retrying...")
        time.sleep(5)
