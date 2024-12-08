import torch

# yolo model
model = torch.hub.load('ultralytics/yolov5', 'yolov5s')

def detect_objects(frame):
    results = model(frame)
    results.render()
    return results.imgs[0]
