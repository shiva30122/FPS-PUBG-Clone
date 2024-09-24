# FPS PUBG Clone

FPS Game Project (Unity)
Overview: This project is a 3D First-Person Shooter (FPS) developed in Unity, designed to provide an immersive gameplay experience while showcasing modular and flexible coding practices.

Features:

Modular System Design: All components, including pickups, inventory, and weapon scripts, are designed to be modular for easy customization and reuse.

Pickups Management: Weapons and items are handled separately from the inventory system. Each weapon type (Gun, Pistol, Melee) has dedicated slots for better organization.

Inventory Manager: This system manages picked items, allowing stacks for identical items and separate entries for different ones. It supports one-time pickups that cannot be dropped and dynamically allocates space for items.

Gun Functionality: A centralized script manages various weapon functionalities, including:

Automatic Shooting
Burst Fire
Shotgun Mechanics
Multi-Point Shooting
Attachments System: The design allows for easy attachment of modules to weapons, enabling dynamic gameplay. For example, attaching a MultiPad Shoot module to an AR gun allows it to fire multiple bullets simultaneously.

Technologies Used:

Unity
C#
Challenges Faced: During development, I encountered challenges such as debugging issues that caused Unity to crash. However, these obstacles strengthened my understanding of critical concepts like coroutines and while loops, enhancing my problem-solving skills.

Future Improvements: Future iterations will include:

Multiplayer or Wi-Fi Connectivity: Implementing multiplayer features for engaging gameplay with friends.
Enhanced Gun Attachments: Expanding the system for more varied and customizable weapon attachments.
Platform Expansion: Developing applications for both Android and PC to reach a broader audience and provide cross-platform gameplay experiences.
