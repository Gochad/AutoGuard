import os
from files import *
from story_mode import *

if __name__ == "__main__":
    cleanup_test_csv_files()

    current_dir = os.path.dirname(os.path.abspath(__file__))
    mods_folder = os.path.join(current_dir, "mods")
    saves_folder = os.path.join(current_dir, "saves")

    gta_installation_path = find_gta_v_installation("C:\\")
    if gta_installation_path:
        prepare_mods_and_scripts(gta_installation_path, mods_folder)

        profiles_folders = find_profiles_folders()
        if profiles_folders:
            copy_saves_to_all_profiles(saves_folder, profiles_folders)

        gta_exe_path = os.path.join(gta_installation_path, "PlayGTAV.exe")
        launch_gta_v_with_working_dir(gta_exe_path)

        
        choose_storymode_if_possible()
        






