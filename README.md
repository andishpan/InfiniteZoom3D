Infinite Zoom 3D
HTW Berlin Praxis Project SS24

Team Members:

  - Andrian - Programming, Level Design, Asset Design
  - Asli - Programming,Assest Design
  - Aaron - Programming, Git Master
  - Tizian- Programming, Project Lead, Level Design
  - Tom - Programming, Design
  - Prof. Tobias Lenz - Project Supervisor 

Overview

The Infinite Zoom 3D Project was developed as part of a university praxis project, in which our team explored procedural generation, visual effects, and game development. The goal was to create an infinite zoom experience where users could explore 3D objects in a continuous zoom, generating an engaging and immersive visual journey.

The project was created using Godot Engine, with a focus on rendering 3D objects and integrating infinite zoom mechanics. We presented our project at the end of the semester, where students and professors had the chance to interact with the setup and provide valuable feedback.
Key Features

  - Infinite Zoom Effect: The core of our project allows users to zoom infinitely into procedurally generated 3D objects, exploring different visual layers.
  - Custom 3D Models and Scenes: We included a variety of custom-designed objects such as mushrooms and fractal-based scenes like the Mandelbrot set.
  - Interactive Elements: Users can interact with different objects and scenes, providing a unique visual and educational experience.
  - Smooth Transitions: Implemented fade-in and fade-out transitions to improve the user experience between different scenes.

Development Journey

1. Initial Setup and Engine Choice

Our project began with a team brainstorming session, where we initially considered Unity for development. However, we later opted for Godot Engine due to its simplicity and flexibility with both GDScript (a Python-like language) and C#. While there were challenges, such as the smaller asset library compared to Unity, Godot's user-friendly interface and accessible learning resources made it a suitable choice for our project.

2. Creating the Zoom Effect

Our first attempt at creating a zoom effect involved basic objects—a sphere, a cube, and a prism—arranged in a scene. We implemented a simple zoom-in effect where the camera moved toward the objects, but we aimed for a more dynamic infinite zoom mechanic.

We eventually developed a procedural tunnel in Blender, textured using noise patterns for a randomized appearance. This tunnel was imported into Godot, and we scripted the camera to create a continuous zoom effect. Additionally, we introduced rotating objects and transitions between different scenes, adding layers of interaction.

3. Challenges with Animation and Scripting

While working with C# in Godot, we encountered documentation limitations. This led to a deeper exploration of GDScript, which we found more flexible for implementing procedural features like wireframe cubes that continuously scale and rotate, creating the desired infinite zoom effect.

4. Collaborations and Contributions

  - Mushrooms: One of our team members designed intricate mushroom models in Blender, which we integrated into the project. These mushrooms became interactive obstacles that the user needed to avoid during gameplay.

  - Transitions and Menus: We developed a 2D main menu and game-over screens, featuring hover effects and animated backgrounds. The main menu included options for starting the zoom experience or entering a relaxation mode where scenes played automatically.

  - Transitions between scenes were enhanced with a fade-in and fade-out animation, ensuring smooth gameplay flow.

5. Final Presentation and Feedback

At the end of the semester, we presented our project at IMI Showtime, where both students and professors could experience the Infinite Zoom 3D project at our booth. The feedback was largely positive, especially regarding the visual elements, procedural zoom mechanics, and the smooth transitions between scenes. Some of the highlights included:

  - Positive Feedback: The endless tunnel and fractal exploration were particularly well-received.
  - Areas for Improvement: Some users suggested that user controls could be made more intuitive, and that the transition times between different scenes could be further optimized.

6. Technologies Used

  - Godot Engine: Used for developing the interactive 3D scenes and procedural zoom effects.
  - Blender: Used for creating 3D assets like mushrooms and tunnels.
  - GDScript & C#: For writing game logic, procedural generation, and scene transitions.

7. How to Run the Project

  - Install Godot: Make sure you have the Godot Engine (v3.2 or later) installed.
  - Clone the Repository:

    bash

    git clone <repository-url>
    cd infinitezoom

  - Open the Project in Godot: Open the project using the project.godot file in the Godot editor.
  - Run the Game: Click the play button to start the infinite zoom experience.

8. Future Work

Based on feedback received during the presentation, we plan the following improvements:

  - Improved Camera Controls: Further refinement of camera controls for a smoother experience.
  - Enhanced UI: Improve the user interface with clearer controls and additional visual cues.
  - Performance Optimization: Enhance performance, especially when rendering complex scenes with multiple objects.

Conclusion

The Infinite Zoom 3D Project allowed us to explore procedural generation, 3D design, and collaborative game development. Despite facing challenges in balancing the procedural elements and user experience, our team worked together to deliver a project that successfully merged interactive mechanics with visually striking scenes.

We are proud of the final outcome and grateful for the feedback from our peers and professors, which helped shape the final version of the project.
