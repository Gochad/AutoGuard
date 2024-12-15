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

if not data.empty:
    data['ElapsedTime'] = (data['Time'] - data['Time'].iloc[0]).dt.total_seconds()
else:
    raise ValueError("No valid data to process.")

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

threshold_distance = 50 
collision_proportion = collision_count / len(data) * 100

print("--- Driving Quality Evaluation ---")
if collision_count > 0:
    print(f"Warning: Detected {collision_count} collisions.")

if data['HitDistance'].min() < threshold_distance:
    print(f"Warning: Minimum distance ({data['HitDistance'].min()} m) fell below safety threshold ({threshold_distance} m).")
else:
    print("Safe distance maintained throughout the drive.")

if collision_proportion > 5:
    print(f"Warning: High collision proportion ({collision_proportion:.2f}%).")
else:
    print("Collision proportion is acceptable.")

print("Analysis complete.")
