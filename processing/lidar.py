import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

filename = 'GTA_LidarData.csv'
columns = [
    'Time', 'SensorPosX', 'SensorPosY', 'SensorPosZ', 'Angle', 'HitDistance', 'HitEntity'
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

data['HitDistance'] = pd.to_numeric(data['HitDistance'], errors='coerce')

if data.empty:
    raise ValueError("No valid data to process.")

data['ElapsedTime'] = (data['Time'] - data['Time'].iloc[0]).dt.total_seconds()

hit_distance_stats = data['HitDistance'].describe()
collision_count = data['HitEntity'].value_counts().get('WorldCollision', 0)
none_count = data['HitEntity'].value_counts().get('None', 0)

print("--- HitDistance Statistics ---")
print(hit_distance_stats)
print(f"Collision count (WorldCollision): {collision_count}")
print(f"No collision count (None): {none_count}")

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['HitDistance'], label='HitDistance', color='blue')
plt.title('Distance from objects over time')
plt.xlabel('Time (s)')
plt.ylabel('Distance (m)')
plt.legend()
plt.grid(True)
plt.show()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['Angle'], label='Angle', color='green')
plt.title('Angle changes over time')
plt.xlabel('Time (s)')
plt.ylabel('Angle (°)')
plt.legend()
plt.grid(True)
plt.show()

collision_types = data['HitEntity'].value_counts()
plt.figure(figsize=(8, 4))
collision_types.plot(kind='bar', color='orange')
plt.title('Collision count by type')
plt.xlabel('Collision Type')
plt.ylabel('Occurrences')
plt.grid(axis='y')
plt.show()

plt.figure(figsize=(10, 6))
plt.plot(data['SensorPosX'], data['SensorPosY'], label='Trajectory', color='purple')
plt.title('Vehicle trajectory')
plt.xlabel('SensorPosX')
plt.ylabel('SensorPosY')
plt.legend()
plt.grid(True)
plt.show()

dx = data['SensorPosX'].diff()
dy = data['SensorPosY'].diff()
dz = data['SensorPosZ'].diff()
dt = data['ElapsedTime'].diff()

dt = dt.replace(0, np.nan)

distance_step = np.sqrt(dx**2 + dy**2 + dz**2)
data['Speed'] = distance_step / dt
data['Acceleration'] = data['Speed'].diff() / dt

speed_stats = data['Speed'].describe()
accel_stats = data['Acceleration'].describe()

print("\n--- Speed Statistics (m/s) ---")
print(speed_stats)
print("\n--- Acceleration Statistics (m/s^2) ---")
print(accel_stats)

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['Speed'], label='Speed (m/s)', color='red')
plt.title('Speed over time')
plt.xlabel('Time (s)')
plt.ylabel('Speed (m/s)')
plt.legend()
plt.grid(True)
plt.show()

plt.figure(figsize=(12, 6))
plt.plot(data['ElapsedTime'], data['Acceleration'], label='Acceleration (m/s^2)', color='cyan')
plt.title('Acceleration over time')
plt.xlabel('Time (s)')
plt.ylabel('Acceleration (m/s^2)')
plt.legend()
plt.grid(True)
plt.show()

data['AngleChange'] = data['Angle'].diff()
angle_sharpness = data['AngleChange'].abs().sum()

print(f"\nTotal angle change (AngleChange): {angle_sharpness:.2f}")

threshold_distance = 50
danger_mask = data['HitDistance'] < threshold_distance
time_in_danger = data.loc[danger_mask, 'ElapsedTime'].diff().sum()

print(f"\nTime spent below {threshold_distance} m distance: {time_in_danger:.2f} s")

collision_proportion = collision_count / len(data) * 100

score = 100

collision_penalty = collision_count * 10
score -= collision_penalty

min_distance = data['HitDistance'].min()
if min_distance < threshold_distance:
    score -= 15

max_speed = data['Speed'].max()
if max_speed > 30:
    score -= 10

angle_sharpness_penalty = angle_sharpness * 0.1
score -= angle_sharpness_penalty

score = max(0, min(score, 100))

print("\n--- Driving Quality Evaluation ---")
if collision_count > 0:
    print(f"Warning: Detected {collision_count} collisions.")
else:
    print("No collisions detected.")

if min_distance < threshold_distance:
    print(f"Warning: Minimum distance ({min_distance:.2f} m) fell below the safety threshold ({threshold_distance} m).")
else:
    print("Safe distance maintained throughout the drive.")

if collision_proportion > 5:
    print(f"Warning: High collision proportion ({collision_proportion:.2f}%).")
else:
    print("Collision proportion is acceptable.")

print(f"Time spent under {threshold_distance} m distance: {time_in_danger:.2f} s")
print(f"Angle changes: {angle_sharpness:.2f}")
print(f"Max speed: {max_speed:.2f} m/s")
print(f"Driving score (0-100): {score:.2f}")

corr_cols = ['HitDistance', 'Speed', 'Acceleration', 'Angle']
corr_cols_existing = [col for col in corr_cols if col in data.columns]
correlation_matrix = data[corr_cols_existing].corr()

print("\n--- Correlation Matrix ---")
print(correlation_matrix)

print("\nAnalysis complete.")
