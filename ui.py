import tkinter as tk
from tkinter import messagebox
import subprocess
import os
import sys

class AutoGuardUI:
    def __init__(self, master):
        self.bg_color = "#2e2e2e" 
        self.fg_color = "#ffffff"
        self.widget_bg = "#3c3c3c"

        self.master = master
        self.master.title("AutoGuard - test environment")
        self.master.geometry("600x500")
        self.master.resizable(False, False)
        self.master.configure(bg=self.bg_color)

        title_label = tk.Label(
            master, 
            text="AutoGuard UI for GTA V", 
            font=("Helvetica", 16, "bold"),
            bg=self.bg_color, 
            fg=self.fg_color
        )
        title_label.pack(pady=10)

        scenario_frame = tk.LabelFrame(
            master, 
            text="  Select test scenarios  ",
            padx=10, 
            pady=10,
            bg=self.bg_color, 
            fg=self.fg_color
        )
        scenario_frame.pack(fill="both", expand=True, padx=15, pady=5)

        self.scenarios = [
            "Test1_easy_drive",
            "Test2_uphill",
            "Test3_train_crossing",
            "Test4_highway_speed",
            "Test5_mountain_road",
            "Test6_highway_speed_with_barrier",
            "Test7_easy_with_pedestrians",
            "Test8_easy_with_random_obstacle"
        ]

        self.scenario_listbox = tk.Listbox(
            scenario_frame,
            selectmode=tk.MULTIPLE, 
            height=8,
            bg=self.widget_bg, 
            fg=self.fg_color,
            highlightbackground=self.bg_color,
            selectbackground="#3399FF",
            selectforeground="#000000"
        )
        self.scenario_listbox.pack(side="left", fill="y")

        for scenario in self.scenarios:
            self.scenario_listbox.insert(tk.END, scenario)

        scrollbar = tk.Scrollbar(scenario_frame, orient="vertical")
        scrollbar.config(command=self.scenario_listbox.yview)
        scrollbar.pack(side="right", fill="y")
        self.scenario_listbox.config(yscrollcommand=scrollbar.set)

        scrollbar.configure(bg=self.bg_color, troughcolor=self.bg_color, activebackground="#666666")

        button_frame = tk.Frame(master, bg=self.bg_color)
        button_frame.pack(pady=10)

        self.launch_button = tk.Button(
            button_frame,
            text="Launch GTA V",
            width=15,
            command=self.launch_gta_v,
            bg="#4c4c4c", 
            fg=self.fg_color,
            activebackground="#666666",
            activeforeground=self.fg_color
        )
        self.launch_button.grid(row=0, column=0, padx=5, pady=5)

        self.analyze_button = tk.Button(
            button_frame,
            text="Analyze results",
            width=15,
            command=self.analyze_results,
            bg="#4c4c4c",
            fg=self.fg_color,
            activebackground="#666666",
            activeforeground=self.fg_color
        )
        self.analyze_button.grid(row=0, column=2, padx=5, pady=5)

        self.log_text = tk.Text(
            master,
            height=10,
            wrap="word",
            bg=self.widget_bg,
            fg=self.fg_color
        )
        self.log_text.pack(fill="both", expand=True, padx=15, pady=5)

        exit_frame = tk.Frame(master, bg=self.bg_color)
        exit_frame.pack(fill="x", padx=15, pady=5)

    def launch_gta_v(self):
        self._save_selected_scenarios()

        self._append_log("Launching GTA V...")
        try:
            subprocess.Popen(["python", "env/app.py"])
            self._append_log("GTA V launched (simulation).")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to launch GTA V: {e}")

    def analyze_results(self):
        documents_path = os.path.join(os.path.expanduser("~"), "Documents")
        output_file = os.path.join(documents_path, "output.txt")

        try:
            with open(output_file, "w") as f:
                subprocess.run(["python", "processing/main.py"], stdout=f, check=True)

            self._append_log(f"Report generated: {output_file}")

            with open(output_file, "r") as f:
                content = f.read()

            self._append_log("\n=== Analysis Results ===")
            self._append_log(content)
            self._append_log("=== End of Results ===\n")

        except Exception as e:
            messagebox.showerror("Error", f"Failed to analyze results: {e}")

    def _save_selected_scenarios(self):
        selected_indices = self.scenario_listbox.curselection()
        if not selected_indices:
            messagebox.showinfo("Info", "No scenarios selected.")
            return

        selected_scenarios = [self.scenario_listbox.get(i) for i in selected_indices]

        output_file = "env/mods/scripts/selectedScenarios.txt"

        try:
            os.makedirs(os.path.dirname(output_file), exist_ok=True)
            with open(output_file, "w") as f:
                for scenario in selected_scenarios:
                    f.write(scenario + "\n")

            self._append_log(f"Selected scenarios saved to '{output_file}':")
            for s in selected_scenarios:
                self._append_log(f"  - {s}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to save selected scenarios: {e}")

    def _append_log(self, text):
        self.log_text.insert(tk.END, text + "\n")
        self.log_text.see(tk.END)

if __name__ == "__main__":
    root = tk.Tk()
    app = AutoGuardUI(root)
    root.mainloop()
