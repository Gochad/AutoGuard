from fpdf import FPDF
import pandas as pd
import matplotlib.pyplot as plt

filename = 'GTA_LidarData_Test7_mountain_road.csv'
columns = [
    'Time', 'PositionX', 'PositionY', 'PositionZ', 'Speed', 'SpeedDeviation',
    'Jerk', 'SteeringAngle', 'LaneOffset', 'LaneDepartures', 'TrafficViolations', 'CollisionDetected'
]

data = pd.read_csv(filename, names=columns, skiprows=1)
data['Time'] = pd.to_datetime(data['Time'], format='%d.%m.%Y %H:%M:%S', errors='coerce')

data['ElapsedTime'] = (data['Time'] - data['Time'].iloc[0]).dt.total_seconds()
data['LaneDepartures'] = pd.to_numeric(data['LaneDepartures'], errors='coerce')

data['LaneDepartures'] = data['LaneDepartures'].fillna(0).astype(int)
data['TrafficViolations'] = pd.to_numeric(data['TrafficViolations'], errors='coerce')
data['TrafficViolations'] = data['TrafficViolations'].fillna(0).astype(int)
data['LaneOffset'] = pd.to_numeric(data['LaneOffset'], errors='coerce')

speed_stats = data['Speed'].describe()
jerk_stats = data['Jerk'].describe()
steering_stats = data['SteeringAngle'].describe()
lane_offset_stats = data['LaneOffset'].describe()

lane_departure_count = data['LaneDepartures'].sum()
traffic_violation_count = data['TrafficViolations'].sum()
collision_count = 0

score = 100
collision_penalty = collision_count * 10
lane_departure_penalty = lane_departure_count * 5
violation_penalty = traffic_violation_count * 3
max_lane_offset = data['LaneOffset'].max()

score -= (collision_penalty + lane_departure_penalty + violation_penalty)
if max_lane_offset > 3.0:
    score -= 10
score = max(0, score)

corr_cols = ['Speed', 'Jerk', 'SteeringAngle', 'LaneOffset']
for col in corr_cols:
    if data[col].dtype == 'bool':
        data[col] = data[col].astype(int)
    else:
        data[col] = pd.to_numeric(data[col], errors='coerce').fillna(0)

correlation_matrix = data[corr_cols].corr()

plots = []
plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['Speed'], label='Speed (m/s)', color='blue')
plt.title('Speed over Time')
plt.xlabel('Time (s)')
plt.ylabel('Speed (m/s)')
plt.legend()
plt.grid(True)
plt.savefig('speed_over_time.png')
plots.append('speed_over_time.png')
plt.close()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['SteeringAngle'], label='Steering Angle (\u00b0)', color='green')
plt.title('Steering Angle over Time')
plt.xlabel('Time (s)')
plt.ylabel('Steering Angle (\u00b0)')
plt.legend()
plt.grid(True)
plt.savefig('steering_angle_over_time.png')
plots.append('steering_angle_over_time.png')
plt.close()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['LaneOffset'], label='Lane Offset (m)', color='orange')
plt.title('Lane Offset over Time')
plt.xlabel('Time (s)')
plt.ylabel('Lane Offset (m)')
plt.legend()
plt.grid(True)
plt.savefig('lane_offset_over_time.png')
plots.append('lane_offset_over_time.png')
plt.close()

plt.figure(figsize=(12, 6))
plt.plot(data['PositionX'], data['PositionY'], label='Trajectory', color='purple')
plt.title('Vehicle Trajectory')
plt.xlabel('PositionX')
plt.ylabel('PositionY')
plt.legend()
plt.grid(True)
plt.savefig('vehicle_trajectory.png')
plots.append('vehicle_trajectory.png')
plt.close()

pdf = FPDF()
pdf.set_auto_page_break(auto=True, margin=15)
pdf.add_page()
pdf.set_font('Arial', 'B', 16)
pdf.cell(0, 10, 'Driving Quality Report', ln=True, align='C')

pdf.set_font('Arial', 'B', 12)
pdf.cell(0, 10, 'Statistics:', ln=True)
pdf.set_font('Arial', '', 10)
pdf.multi_cell(0, 10, f"""
--- Speed Statistics ---
{speed_stats.to_string()}

--- Jerk Statistics ---
{jerk_stats.to_string()}

--- Steering Angle Statistics ---
{steering_stats.to_string()}

--- Lane Offset Statistics ---
{lane_offset_stats.to_string()}
""")

pdf.set_font('Arial', 'B', 12)
pdf.cell(0, 10, 'Event Counts:', ln=True)
pdf.set_font('Arial', '', 10)
pdf.multi_cell(0, 10, f"""
Lane Departures: {lane_departure_count}
Traffic Violations: {traffic_violation_count}
Collisions: {collision_count}

Driving Score (0-100): {score}
""")

pdf.set_font('Arial', 'B', 12)
pdf.cell(0, 10, 'Correlation Matrix:', ln=True)
pdf.set_font('Arial', '', 10)
pdf.multi_cell(0, 10, correlation_matrix.to_string())

for plot in plots:
    pdf.add_page()
    pdf.image(plot, x=10, y=30, w=190)

output_path = 'report.pdf'
pdf.output(output_path)