import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

filename = 'NewData.csv'
columns = [
    'Time', 'PositionX', 'PositionY', 'PositionZ', 'Speed', 'SpeedDeviation',
    'Jerk', 'SteeringAngle', 'LaneOffset', 'LaneDepartures', 'TrafficViolations', 'CollisionDetected'
]

try:
    data = pd.read_csv(
        filename,
        names=columns,
        skiprows=1
    )

    data['Time'] = pd.to_datetime(data['Time'], format='%d.%m.%Y %H:%M:%S', errors='coerce')
    
    print("Data loaded successfully.")
except Exception as e:
    print(f"Error loading data: {e}")
    exit()

if data.empty:
    raise ValueError("No valid data to process.")

data['ElapsedTime'] = (data['Time'] - data['Time'].iloc[0]).dt.total_seconds()

speed_stats = data['Speed'].describe()
jerk_stats = data['Jerk'].describe()
steering_stats = data['SteeringAngle'].describe()
lane_offset_stats = data['LaneOffset'].describe()

print("\n--- Speed Statistics ---")
print(speed_stats)

print("\n--- Jerk Statistics ---")
print(jerk_stats)

print("\n--- Steering Angle Statistics ---")
print(steering_stats)

print("\n--- Lane Offset Statistics ---")
print(lane_offset_stats)

lane_departure_count = data['LaneDepartures'].sum()
traffic_violation_count = data['TrafficViolations'].sum()
collision_count = data['CollisionDetected'].sum()

print("\n--- Event Counts ---")
print(f"Lane Departures: {lane_departure_count}")
print(f"Traffic Violations: {traffic_violation_count}")
print(f"Collisions: {collision_count}")

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['Speed'], label='Speed (m/s)', color='blue')
plt.title('Speed over Time')
plt.xlabel('Time (s)')
plt.ylabel('Speed (m/s)')
plt.legend()
plt.grid(True)
plt.show()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['SteeringAngle'], label='Steering Angle (°)', color='green')
plt.title('Steering Angle over Time')
plt.xlabel('Time (s)')
plt.ylabel('Steering Angle (°)')
plt.legend()
plt.grid(True)
plt.show()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['LaneOffset'], label='Lane Offset (m)', color='orange')
plt.title('Lane Offset over Time')
plt.xlabel('Time (s)')
plt.ylabel('Lane Offset (m)')
plt.legend()
plt.grid(True)
plt.show()

min_lane_offset = data['LaneOffset'].min()
max_lane_offset = data['LaneOffset'].max()

print(f"\nMin Lane Offset: {min_lane_offset}")
print(f"Max Lane Offset: {max_lane_offset}")

plt.figure(figsize=(12, 6))
plt.plot(data['PositionX'], data['PositionY'], label='Trajectory', color='purple')
plt.title('Vehicle Trajectory')
plt.xlabel('PositionX')
plt.ylabel('PositionY')
plt.legend()
plt.grid(True)
plt.show()

score = 100

collision_penalty = collision_count * 10
score -= collision_penalty

lane_departure_penalty = lane_departure_count * 5
score -= lane_departure_penalty

violation_penalty = traffic_violation_count * 3
score -= violation_penalty

if max_lane_offset > 3.0:
    score -= 10

score = max(0, score)

print("\n--- Driving Quality Evaluation ---")
print(f"Lane Departures: {lane_departure_count}")
print(f"Traffic Violations: {traffic_violation_count}")
print(f"Collisions: {collision_count}")
print(f"Driving Score (0-100): {score}")

corr_cols = ['Speed', 'Jerk', 'SteeringAngle', 'LaneOffset']
correlation_matrix = data[corr_cols].corr()

print("\n--- Correlation Matrix ---")
print(correlation_matrix)

print("\nAnalysis complete.")
