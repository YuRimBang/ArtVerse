import pandas as pd
import os
import glob

input_dir = "C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/House"
output_dir = "C:/Users/LEMON AB/Desktop/ArtVerse/Project/Data_Parsing/House_yolo_file"
os.makedirs(output_dir, exist_ok=True)

label_mapping = {
    "굴뚝": 0,
    "길": 1,
    "꽃": 2,
    "나무": 3,
    "문": 4,
    "산": 5,
    "연기": 6,
    "연못": 7,
    "울타리": 8,
    "잔디": 9,
    "지붕": 10,
    "집벽": 11,
    "집전체": 12,
    "창문": 13,
    "태양": 14
}

def convert_to_yolo(row, img_width=1280, img_height=1280):
    class_id = label_mapping.get(row['label'], -1)
    if class_id == -1:
        print(f"Warning: Unknown label '{row['label']}'")
        return None
    x_center = (row['x'] + row['w'] / 2) / img_width
    y_center = (row['y'] + row['h'] / 2) / img_height
    width = row['w'] / img_width
    height = row['h'] / img_height
    return f"{class_id} {x_center:.6f} {y_center:.6f} {width:.6f} {height:.6f}"

csv_files = glob.glob(os.path.join(input_dir, "*.csv"))

for csv_file in csv_files:
    df = pd.read_csv(csv_file)
    base_name = os.path.splitext(os.path.basename(csv_file))[0]

    for img_id, group in df.groupby('img_id'):
        output_file = os.path.join(output_dir, f"{img_id}.txt")
        with open(output_file, 'a') as f:
            for _, row in group.iterrows():
                yolo_line = convert_to_yolo(row)
                if yolo_line:
                    f.write(yolo_line + '\n')

print(f"YOLO 형식 파일이 {output_dir}에 저장되었습니다.")
