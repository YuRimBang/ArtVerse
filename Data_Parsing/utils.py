### python=3.10 , cuda 11.8,
### conda install pytorch==2.5.0 torchvision==0.20.0 torchaudio==2.5.0  pytorch-cuda=11.8 -c pytorch -c nvidia
### tensorflow==2.13
### pip install ulralytics

# from tensorflow.python.client import device_lib
# print(device_lib.list_local_devices())

import os
os.environ["KMP_DUPLICATE_LIB_OK"]="TRUE"
import torch

print(torch.cuda.get_device_name(0))
print(torch.cuda.is_available())

# NVIDIA GeForce RTX 4060 Laptop GPU
# True



