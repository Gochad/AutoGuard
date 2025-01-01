import pandas as pd
import numpy as np

def count_continuous_ones(series):
    binary = (series > 0).astype(int)
    shifted = binary.shift(1, fill_value=0)
    return ((binary == 1) & (shifted == 0)).sum()

def generate_report_console(filename='GTA_LidarData_Test.csv'):
    columns = [
        'Time', 'PositionX', 'PositionY', 'PositionZ', 'Speed', 'SpeedDeviation',
        'Jerk', 'SteeringAngle', 'LaneOffset', 'LaneDepartures', 
        'TrafficViolations', 'CollisionDetected'
    ]

    data = pd.read_csv(filename, names=columns, skiprows=1, delimiter=';')
    data['Time'] = pd.to_datetime(data['Time'], format='%d.%m.%Y %H:%M:%S', errors='coerce')
    data['ElapsedTime'] = (data['Time'] - data['Time'].iloc[0]).dt.total_seconds()
    
    numeric_cols = ['Speed', 'SpeedDeviation', 'Jerk', 'SteeringAngle', 
                    'LaneOffset', 'LaneDepartures', 'TrafficViolations']
    for col in numeric_cols:
        data[col] = pd.to_numeric(data[col], errors='coerce').fillna(0)

    data['LaneDepartures'] = data['LaneDepartures'].astype(int)
    data['TrafficViolations'] = data['TrafficViolations'].astype(int)
    data['CollisionDetected'] = (data['CollisionDetected'] > 0).astype(int)

    total_time = data['ElapsedTime'].iloc[-1] - data['ElapsedTime'].iloc[0]
    avg_speed = data['Speed'].mean()
    distance_m = avg_speed * total_time
    distance_km = distance_m / 1000.0

    speed_array = data['Speed'].to_numpy()
    accel_array = np.diff(speed_array)
    max_acceleration = np.max(accel_array) if len(accel_array) > 0 else 0
    max_braking = np.min(accel_array) if len(accel_array) > 0 else 0

    jerk_mean = data['Jerk'].mean()
    jerk_std = data['Jerk'].std()
    is_smooth = (jerk_std < 1.0) and (jerk_mean < 0.5)

    lane_departure_count = count_continuous_ones(data['LaneDepartures'])
    traffic_violation_count = count_continuous_ones(data['TrafficViolations'])
    collision_count = count_continuous_ones(data['CollisionDetected'])

    max_lane_offset = data['LaneOffset'].max()

    collision_penalty = collision_count * 10
    lane_departure_penalty = lane_departure_count * 5
    violation_penalty = traffic_violation_count * 3

    lane_offset_penalty = 0
    if max_lane_offset > 3.0:
        lane_offset_penalty += 10

    score = 100
    score -= (collision_penalty + lane_departure_penalty + violation_penalty + lane_offset_penalty)
    if jerk_std > 2.0:
        score -= 10
    score = max(0, score)

    print("\n========= DRIVING QUALITY REPORT =========")
    print(f"Filename: {filename}")
    print("-----------------------------------------")
    print(f"Total Distance (km): {distance_km:.2f}")
    print(f"Average Speed (m/s): {avg_speed:.2f}")
    print(f"Max Acceleration (m/s^2): {max_acceleration:.2f}")
    print(f"Max Braking (m/s^2): {max_braking:.2f}")
    print(f"Mean Jerk: {jerk_mean:.2f}")
    print(f"Jerk Std Dev: {jerk_std:.2f}")
    print(f"Smooth Trajectory?: {'Yes' if is_smooth else 'No'}")
    print(f"\nContinuous Lane Departures (runs of 1): {lane_departure_count}")
    print(f"Continuous Traffic Violations (runs of 1): {traffic_violation_count}")
    print(f"Continuous Collisions (runs of 1): {collision_count}")
    print(f"Peak Lane Offset: {max_lane_offset:.2f}")
    print(f"\nRefined Driving Score: {score}")
    print("==========================================\n")

if __name__ == '__main__':
    generate_report_console('GTA_LidarData_Test.csv')