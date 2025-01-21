import os
os.environ['KMP_DUPLICATE_LIB_OK'] = 'TRUE'  # 가장 상단에 위치

import torch
torch.set_num_threads(1)

from ultralytics import YOLO as yolo


def house_train_model():
    path_house = 'house_data.yaml'
    model_house = yolo('yolov8n.pt')
    
    # 모델 학습
    model_house.train(data=path_house, epochs=1, device="cuda", batch=16)
    
    model_house.export(format='onnx', opset=9, dynamic=True)

def man_train_model():
    path_man = 'man_data.yaml'
    model_man = yolo('yolov8n.pt')

    # 모델 학습
    model_man.train(data=path_man, epochs=10, device="cuda", batch=16)
    
    model_path = 'C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/man/detect/train/weights/man_model_barr.onnx'
    x = torch.randn(1, 3, 640, 640)  # 모델 입력 텐서
    torch.onnx.export(model_man.model,  # 모델 객체
                      x,                 # 모델 입력
                      model_path,        # 저장 경로
                      export_params=True,  # 학습된 파라미터를 모델 파일에 저장
                      opset_version=9,     # 사용할 ONNX 버전
                      do_constant_folding=True,  # 상수 폴딩을 통한 최적화 수행 여부
                      input_names=['images'],   # 모델의 입력 이름
                      output_names=['output0'])  # 모델의 출력 이름

def woman_train_model():
    path_woman = 'woman_data.yaml'
    model_woman = yolo('yolov8n.pt')
    
    # 모델 학습
    model_woman.train(data=path_woman, epochs=10, device="cuda", batch=16)
    
    model_path = 'C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/woman/detect/train/weights/woman_model_barr.onnx'
    x = torch.randn(1, 3, 640, 640)  # 모델 입력 텐서
    torch.onnx.export(model_woman.model,  # 모델 객체
                      x,                 # 모델 입력
                      model_path,        # 저장 경로
                      export_params=True,  # 학습된 파라미터를 모델 파일에 저장
                      opset_version=9,     # 사용할 ONNX 버전
                      do_constant_folding=True,  # 상수 폴딩을 통한 최적화 수행 여부
                      input_names=['images'],   # 모델의 입력 이름
                      output_names=['output0'])  # 모델의 출력 이름


def tree_train_model():
    path_tree = 'tree_data.yaml'
    model_tree = yolo('yolov8n.pt')
    
    # 모델 학습
    model_tree.train(data=path_tree, epochs=10, device="cuda", batch=16)
    
    # 학습이 완료된 후 모델을 ONNX 형식으로 내보내기
    model_path = 'C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/tree/detect/train/weights/tree_model_barr.onnx'
    x = torch.randn(1, 3, 640, 640)  # 모델 입력 텐서
    torch.onnx.export(model_tree.model,  # 모델 객체
                      x,                 # 모델 입력
                      model_path,        # 저장 경로
                      export_params=True,  # 학습된 파라미터를 모델 파일에 저장
                      opset_version=9,     # 사용할 ONNX 버전
                      do_constant_folding=True,  # 상수 폴딩을 통한 최적화 수행 여부
                      input_names=['images'],   # 모델의 입력 이름
                      output_names=['output0'])  # 모델의 출력 이름

if __name__ == '__main__':
    #tree_train_model()
    house_train_model()
    #man_train_model() 3
    #woman_train_model() 4

