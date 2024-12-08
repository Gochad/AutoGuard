import os
import shutil
import subprocess

def find_gta_v_installation(start_dir="C:\\"):
    """
    Searches for the GTA V folder on the specified drive.
    """
    print("Searching for the GTA V installation... This might take a while.")
    for root, dirs, files in os.walk(start_dir):
        for file in files:
            if file.lower() == "playgtav.exe":
                print(f"GTA V installation found at: {root}")
                return root

    print("GTA V installation not found.")
    return None


def copy_or_merge_folder(src_folder, dst_folder):
    """
    Copies all files and folders from src_folder to dst_folder. If dst_folder does not exist, it is created.
    """
    if not os.path.exists(dst_folder):
        os.makedirs(dst_folder)
    for item in os.listdir(src_folder):
        src_path = os.path.join(src_folder, item)
        dst_path = os.path.join(dst_folder, item)
        if os.path.isdir(src_path):
            shutil.copytree(src_path, dst_path, dirs_exist_ok=True)
        else:
            shutil.copy2(src_path, dst_path)


def prepare_mods_and_scripts(gta_installation_path, mods_folder):
    """
    Prepares mods and scripts for GTA V.
    """
    gta_mods_folder = gta_installation_path
    gta_scripts_folder = os.path.join(gta_installation_path, "scripts")

    print("Copying mods to GTA V folder...")
    copy_or_merge_folder(mods_folder, gta_mods_folder)

    scripts_folder = os.path.join(mods_folder, "scripts")
    if os.path.exists(scripts_folder):
        print("Copying scripts to GTA V/scripts folder...")
        copy_or_merge_folder(scripts_folder, gta_scripts_folder)


def launch_gta_v_with_working_dir(game_path):
    """
    Launches GTA V with the correct working directory.
    """
    if game_path and os.path.exists(game_path):
        working_dir = os.path.dirname(game_path)
        print(f"Launching GTA V from: {game_path}")
        try:
            subprocess.Popen(f'"{game_path}"', cwd=working_dir, shell=True)
        except Exception as e:
            print(f"Failed to launch GTA V. Error: {e}")
    else:
        print(f"File not found: {game_path}")


def find_profiles_folders():
    """
    Recursively searches for all 'Profiles' folders in the Documents directory for GTA V.
    """
    profiles = []
    documents_dir = os.path.join(os.environ["USERPROFILE"], "Documents")
    
    for root, dirs, files in os.walk(documents_dir):
        if "Profiles" in dirs:
            profiles_path = os.path.join(root, "Profiles")
            profiles.append(profiles_path)
            print(f"Found 'Profiles' folder: {profiles_path}")
    
    if not profiles:
        print("No 'Profiles' folder found.")
    return profiles


def copy_saves_to_all_profiles(saves_folder, profiles_folders):
    """
    Copies save files to all GTA V profile folders.
    """
    if not os.path.exists(saves_folder):
        print(f"Saves folder not found: {saves_folder}")
        return

    for profile_folder in profiles_folders:
        if not os.path.exists(profile_folder):
            print(f"Profile folder not found: {profile_folder}")
            continue

        print(f"Copying saves from {saves_folder} to {profile_folder}...")
        for item in os.listdir(saves_folder):
            src_path = os.path.join(saves_folder, item)
            dst_path = os.path.join(profile_folder, item)
            shutil.copy2(src_path, dst_path)
        print(f"Saves copied successfully to {profile_folder}.")
