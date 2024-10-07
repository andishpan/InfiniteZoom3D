import numpy as np
from PIL import Image


def getHsvColorFromIteration(iteration, max_iteration):
    if(iteration == max_iteration):
        return (0, 0, 0)
    hue = 255 * iteration / max_iteration
    return (hue, 255, 255)


width, height = 100, 100

x0, x1, y0, y1 = -2.0, 0.47, -1.12, 1.12

image = np.zeros((height, width, 3), dtype=np.uint8)

max_iteration = 100

for Px in range(width):
    for Py in range(height):
        x0_scaled = x0 + (x1 - x0) * Px / width
        y0_scaled = y0 + (y1 - y0) * Py / height
        x, y = 0.0, 0.0
        iteration = 0
        while (x*x + y*y <= 4) and (iteration < max_iteration):
            xtemp = x*x - y*y + x0_scaled
            y = 2*x*y + y0_scaled
            x = xtemp
            iteration += 1
        
        image[Py, Px] = getHsvColorFromIteration(iteration, max_iteration)

image = Image.fromarray(image, 'HSV').convert('RGB')
image.show()

