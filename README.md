# CharacterController2D

CharacterController2D is a reimplementation of Unity's CharacterController class using the 2D physics engine that doesn't share the same limitations of other solutions.

NOTE: CharacterController2D.Move() should be called in FixedUpdate().

## Features

- Behaves almost exactly like CharacterController.

- Not raycast-based, uses Rigidbody2D.Cast() therefore takes into account the exact shape of your character's collider.

- Only uses 2 calls to Rigidbody2D.Cast() per Move() call so is highly performant.

- One way platforms with any collider type (BoxCollider, PolygonCollider, etc...) using only a tag.

- Supports ignoring one way platforms for a single frame or extended periods (useful for dropping through platforms).

- Supports "sliding" along the ground normal for smooth slope traversial.

- Slope limit for ground and ceiling.

- Blocker mask allows you to set what layers affect movement.

## Setup

Add a CharacterController2D to your player object and it will add a Rigidbody2D component and configure it to it's optimal defaults.

You will need to add a collider to your player. It is recommended you use a BoxCollider or PolygonCollider since if you have slopes in your collider that are steeper than slopeLimit then you may have issues with isGrounded and sliding along the ground normal.

## Usage

You can use CharacterController2D the same way you would use a CharacterController with a few additional features and the omission of SimpleMove().

In addition to isGrounded, CharacterController2D provides a few other useful values.

- isOnCeiling
- isOnOneWayPlatform
- groundNormal

## Licence

[The Unlicence](http://unlicense.org/), you can use CharacterController2D in anyway you see fit however I would love to hear from you if you use it in your project!