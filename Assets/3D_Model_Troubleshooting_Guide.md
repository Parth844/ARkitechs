# 3D Model Display Troubleshooting Guide

## Issue: 3D Models Not Showing in AR Scene

### Quick Debug Steps

1. **Test with Simple Cube First**
   - Use the new `LoadTestCube()` method to verify basic 3D rendering works
   - This will help isolate if the issue is with model loading or general 3D rendering

2. **Check Console Logs**
   - Look for debug messages starting with 📂 and ✅
   - The enhanced logging will show exactly where the loading process fails

3. **Verify File Paths**
   - Check that model files exist in `StreamingAssets/Models/`
   - Ensure file names match exactly (case-sensitive)

### Common Issues and Solutions

#### 1. Model Scale Too Small
**Problem**: Model loads but is invisible due to extremely small scale
**Solution**: 
- Current scale is set to `0.01f` (1% of original size)
- Try increasing `defaultScale` to `0.1f` or `1.0f` in LocalModelManager
- Check if model appears when looking very close to spawn position

#### 2. Model Position Outside Camera View
**Problem**: Model spawns but camera can't see it
**Solution**:
- Current spawn position is `(0, 0, 1f)` - 1 meter in front of camera
- Try adjusting `spawnPosition` to `(0, 0, 2f)` for better visibility
- Ensure camera is looking in the right direction

#### 3. Missing Materials/Shaders (URP Issue)
**Problem**: Model loads but has no visible materials
**Solution**:
- **IMPORTANT**: This project uses Universal Render Pipeline (URP)
- Standard shaders won't work - use URP shaders instead
- Try: `Universal Render Pipeline/Lit` or `Universal Render Pipeline/Unlit`
- Check if models have URP-compatible materials assigned
- GLTFast should handle URP conversion automatically, but verify

#### 4. AR Camera Setup Issues
**Problem**: AR camera not properly configured
**Solution**:
- Ensure ARCameraManager and ARCameraBackground components are attached
- Check that camera is tagged as "MainCamera"
- Verify AR Foundation packages are properly installed

#### 5. GLTFast Loading Issues
**Problem**: GLB files fail to load
**Solution**:
- Verify GLTFast package is installed (com.unity.cloud.gltfast)
- Check that .glb files are valid (not corrupted)
- Try with simpler models first

### Debug Methods Added

1. **Enhanced Logging**: Detailed console output for each loading step
2. **File Existence Check**: Verifies model files exist before loading
3. **Renderer Analysis**: Checks if loaded models have visible renderers
4. **Test Cube Method**: `LoadTestCube()` for basic 3D rendering test

### Testing Steps

1. **Run the scene** and check console for debug messages
2. **Try LoadTestCube()** first to verify 3D rendering works
3. **Check camera position** - look around spawn area (0,0,1)
4. **Adjust scale** if model is too small/large
5. **Verify UI connections** - ensure buttons are properly wired

### File Locations

- **Models**: `Assets/StreamingAssets/Models/`
- **Scripts**: `Assets/script/intraction/`
- **Main Scene**: `Assets/Scenes/intraction.unity`

### URP-Specific Issues

**Render Pipeline**: This project uses Universal Render Pipeline (URP)
- **Test Cube**: Now uses `Universal Render Pipeline/Lit` shader
- **GLB Models**: Should auto-convert to URP materials via GLTFast
- **If models appear black**: Likely shader compatibility issue

**URP Shader Fallback**:
```csharp
// If models appear but are black/unlit, try this in code:
var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
material.color = Color.white;
renderer.material = material;
```

### Next Steps

If the test cube works but models don't:
1. Check model file integrity
2. Verify GLTFast URP compatibility
3. Check if models need URP material conversion

If nothing appears at all:
1. Check camera setup
2. Verify AR Foundation configuration
3. Check URP render pipeline settings
4. Try the test cube first to verify basic 3D rendering
