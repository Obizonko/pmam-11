from torchvision import models

def create_model(outputchannels=1, aux_loss=False, freeze_backbone=False):
    model = models.segmentation.deeplabv3_resnet101(
        weights=None, 
        progress=True, 
        num_classes=outputchannels, 
        aux_loss=aux_loss
    )

    if freeze_backbone:
        for param in model.backbone.parameters():
            param.requires_grad = False

    return model
