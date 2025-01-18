import tkinter as tk
from tkinter import ttk, messagebox
import subprocess
import os
import sys

class AutoGuardUI:
    def __init__(self, master):
        self.master = master
        self.master.title("AutoGuard - Test Environment")

        self.master.geometry("600x500")
        self.master.resizable(False, False)

        title_label = tk.Label(master, text="AutoGuard UI for GTA V", font=("Helvetica", 16, "bold"))
        title_label.pack(pady=10)

        scenario_frame = tk.LabelFrame(master, text="Test Scenarios", padx=10, pady=10)
        scenario_frame.pack(fill="x", padx=15, pady=5)

        # tk.Label(scenario_frame, text="Choose a scenario:").pack(side="left")
        # self.scenarios = ["City Test", "Highway Test", "Mixed Traffic Test"]
        # self.scenario_var = tk.StringVar(value=self.scenarios[0])
        
        scenario_dropdown = ttk.Combobox(scenario_frame, textvariable=self.scenario_var, values=self.scenarios, state="readonly")
        scenario_dropdown.pack(side="left", padx=5)

        button_frame = tk.Frame(master)
        button_frame.pack(pady=10)

        self.launch_button = tk.Button(button_frame, text="Launch GTA V", width=15, command=self.launch_gta_v)
        self.launch_button.grid(row=0, column=0, padx=5, pady=5)

        self.analyze_button = tk.Button(button_frame, text="Analyze Results", width=15, command=self.analyze_results)
        self.analyze_button.grid(row=0, column=2, padx=5, pady=5)

        self.log_text = tk.Text(master, height=10, wrap="word")
        self.log_text.pack(fill="both", expand=True, padx=15, pady=5)

        exit_frame = tk.Frame(master)
        exit_frame.pack(fill="x", padx=15, pady=5)

    def launch_gta_v(self):
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

    def _append_log(self, text):
        self.log_text.insert(tk.END, text + "\n")
        self.log_text.see(tk.END)

if __name__ == "__main__":
    root = tk.Tk()
    app = AutoGuardUI(root)
    root.mainloop()
