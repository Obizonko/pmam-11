import glob
import os

import cv2
import numpy as np
import torch
from PIL import Image
from torch.utils.data import DataLoader, Dataset, SubsetRandomSampler
from torchvision.transforms import (Compose, Normalize, Resize, ToPILImage, ToTensor)
import transforms as T


class SegmentationDataset(Dataset):
    def __init__(self, folder_path, transforms):
        super(SegmentationDataset, self).__init__()
        self.images = glob.glob(os.path.join(folder_path, 'images', '*.png'))  # Зміна на png
        self.masks = [os.path.join(folder_path, 'masks', os.path.basename(image)) for image in self.images]
        self.transforms = transforms

        assert (len(self.images) == len(self.masks))

    def __getitem__(self, index):
        img_path = self.images[index]
        mask_path = self.masks[index]

        img = Image.open(img_path).convert('RGB')
        mask = Image.open(mask_path).convert('L') 

        if self.transforms is not None:
            img, mask = self.transforms(img, mask)

        return img, mask

    def __len__(self):
        return len(self.images)

def get_dataset(image_set, transform, dataset_dir):
    return SegmentationDataset(folder_path=os.path.join(dataset_dir, image_set), transforms=transform)

def get_transform(train):
    base_size = 520
    crop_size = 480

    min_size = int((0.5 if train else 1.0) * base_size)
    max_size = int((2.0 if train else 1.0) * base_size)
    transforms = []
    transforms.append(T.RandomResize(min_size, max_size))
    if train:
        transforms.append(T.RandomColorJitter(
            brightness=0.25, contrast=0.25, saturation=0.25, hue=0.25))
        transforms.append(T.RandomGaussianSmoothing(radius=[0, 5]))
        transforms.append(T.RandomRotation(degrees=30, fill=0))
        transforms.append(T.RandomHorizontalFlip(0.5))
        transforms.append(T.RandomPerspective(fill=0))
        transforms.append(T.RandomCrop(crop_size, fill=0))
        transforms.append(T.RandomGrayscale(p=0.1))
    transforms.append(T.ToTensor())
    transforms.append(T.Normalize(mean=[0.485, 0.456, 0.406],
                                  std=[0.229, 0.224, 0.225]))

    return T.Compose(transforms)
