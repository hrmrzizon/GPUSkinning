# GPUSkinning

GPUSkinning is made for performance of extensive skinning object. This repositoy forked from [chengkehan](https://github.com/chengkehan)'s repository.

# Current Features

 - Sampling Animation clips in Unity's Animation System(mecanim's Animator)
 - Processing vertex deformation in vertex shader. Support Unlit, Specular, Metalic shader as Surface Shader.

# Improvements in Future

Source repository has few Ineffciency things.  

 - Matrix data for vertex deformation exist as form of ordinary binary data. This has three layer memory allocation. first, load byte array form disk(allocation too). second, convert to texture from byte array. third, allocation in vram for texture. so vertex deformation data must be converted to texture(handled by Unity). 
 - GPUSkinningPlayerMono gernerate Unity's Texture object per component, This waste vast memories.
 - Few code in animation sampling script is deprecated.
 - Matrix data in GPUSkinningAnimation's each clip waste system resource(memory, load time, etc..). It must be converted to Unity's Texture or deleted.
