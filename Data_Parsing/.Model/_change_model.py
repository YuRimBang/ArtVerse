import os

#중복 라이브러리 초기화를 허용
os.environ['KMP_DUPLICATE_LIB_OK'] = 'TRUE'

from ultralytics import YOLO as yolo
import onnx
from onnx import shape_inference, helper, checker

# model = yolo("C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/house/detect/train/weights/best.pt")
# model.export(format="onnx")

#-------------------------------------------------------------------------------
# import onnx
# from onnx import helper, checker

# # 모델 로드
# model_path = "house/detect/train/weights/best.onnx"
# model = onnx.load(model_path)

# # 그래프에서 Split 노드 찾기 및 "split" 속성 제거
# new_nodes = []
# for node in model.graph.node:
#     if node.op_type == "Split":
#         # 기존 attribute에서 "split" 속성 제거
#         new_attributes = [attr for attr in node.attribute if attr.name != "split"]

#         # 새로운 split attribute 생성 (정수형 배열로 변환)
#         new_split_values = [2, 2]  # 예시로 두 부분으로 나누기
#         try:
#             # Ensure that split values are valid integers
#             new_split_values = list(map(int, new_split_values))
#             new_attributes.append(helper.make_attribute("split", new_split_values))
#         except ValueError as e:
#             print(f"Error converting split attribute values: {e}")
#             continue

#         # 새로운 노드를 생성하고 새로운 attribute 할당
#         new_node = onnx.helper.make_node(
#             node.op_type, node.input, node.output, attributes=new_attributes
#         )
#         new_nodes.append(new_node)
#     else:
#         new_nodes.append(node)

# # 모델의 그래프에 새로운 노드들 추가
# model.graph.ClearField('node')
# model.graph.node.extend(new_nodes)

# 모델 검증 및 저장
# try:
#     checker.check_model(model)
#     print("ONNX 모델 검증 완료!")
#     onnx.save(model, "house/detect/train/weights/best_house_barr.onnx")
#     print("변환 완료")
# except onnx.checker.ValidationError as e:
#     print(f"모델 검증 실패: {e}")
## ---------------------------------------------------------------------------------



# def convert_nchw_to_nhwc(model):
#     for node in model.graph.node:
#         for attr in node.attribute:
#             if attr.name == "axes" and list(attr.ints) == [1]:  # NCHW -> NHWC 변환 체크
#                 try:
#                     attr.ints[:] = [3]  # axes 업데이트 (NCHW -> NHWC 변환을 위한 준비)
#                 except Exception as e:
#                     print(f"Node {node.name}에서 axes 업데이트 중 오류 발생: {e}")
#     return model

# # 모델 경로 설정
# model_path = "C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/runs/detect/train7/weights/best.onnx"
# inferred_model_path = "C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/runs/detect/train7/weights/best_barr3.onnx"
# nhwc_model_path = "C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/.Model/runs/detect/train7/weights/best_barr4.onnx"

# try:
#     # 모델 로드
#     model = onnx.load(model_path)

#     # Shape 추론 실행
#     inferred_model = shape_inference.infer_shapes(model)

#     # 추론된 모델 저장
#     onnx.save(inferred_model, inferred_model_path)
#     print(f"추론된 모델 저장 완료: {inferred_model_path}")

#     # NHWC 변환
#     model = onnx.load(inferred_model_path)
#     nhwc_model = convert_nchw_to_nhwc(model)

#     # 모델 검증
#     checker.check_model(nhwc_model)
#     print("ONNX 모델 검증 완료!")

#     # NHWC 변환된 모델 저장
#     onnx.save(nhwc_model, nhwc_model_path)
#     print(f"NHWC 변환된 모델 저장 완료: {nhwc_model_path}")

# except onnx.checker.ValidationError as e:
#     print(f"모델 검증 실패: {e}")
# except Exception as e:
#     print(f"예기치 않은 오류 발생: {e}")


#---------------------------------------------------------------------
# model = onnx.load("runs/detect/train9/weights/best.onnx")

# # Split 노드 수정
# for node in model.graph.node:
#     if node.op_type == "Split" and len(node.input) == 1:  # split 입력이 없는 경우
#         split_name = f"{node.name}_split"

#         # 분할 값을 지정 (예: 2개의 출력으로 분할)
#         split_tensor = helper.make_tensor(
#             name=split_name,
#             data_type=onnx.TensorProto.INT64,
#             dims=[2],  # 분할 크기 개수
#             vals=[2, 2],  # 분할 크기 값
#         )

#         # 모델 그래프에 split 입력 텐서 추가
#         model.graph.initializer.append(split_tensor)

#         # Split 노드에 split 입력 추가
#         node.input.append(split_name)

# # 모델 검증 및 저장
# try:
#     checker.check_model(model)
#     print("ONNX 모델 검증 완료!")
#     onnx.save(model, "runs/detect/train9/weights/best_house_barr.onnx")
#     print("변환 완료!")
# except onnx.checker.ValidationError as e:
#     print(f"모델 검증 실패: {e}")

# ONNX 모델 로드
model_path = "runs/detect/train9/weights/best_house_barr_fixed.onnx"
model = onnx.load(model_path)

# 그래프의 노드 순회
for node in model.graph.node:
    if node.op_type == "Split":
        # split 속성이 없다면 추가
        if not any(attr.name == "split" for attr in node.attribute):
            split_values = [1, 1]  # 원하는 split 값으로 변경
            split_attr = helper.make_attribute("split", split_values)
            node.attribute.append(split_attr)

# 수정된 모델 저장
fixed_model_path = "runs/detect/train9/weights/best_house_barr_fixed1.onnx"
onnx.save(model, fixed_model_path)
print(f"수정된 모델 저장 완료: {fixed_model_path}")