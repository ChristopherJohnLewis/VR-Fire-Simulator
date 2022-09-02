//Various blending functions and structs to generate terrain maps and textures

//struct for packing data
struct VisualData {
	sampler2D rockHeight;
	sampler2D burnMap;
	sampler2D vegMap;
	sampler2D satMap;
	half rockControl;
	half rockPower;
	float burnProgress;
	float maxBurnVal;
	float fireFrontSize;
	float fireFrontPos;
	fixed4 fireFrontColor;
};

struct BurnState {
	float burnState;
	float fireLine;
};

//globally declared variables
sampler2D _BurnMap;
fixed3 _RelativePosition;
float _BurnProgress;
sampler2D _PerlinNoise;

//helper function for smoothly blending height map edges
float sigmoid(float power, float midpoint, float control) {
	return 1.0 / (1 + pow(10, -power * (control - midpoint)));
}

// Blend between two textures using a height map
fixed4 heightBlend(half4 tex1, half4 tex2, half power, half sigmoidPower, half control, sampler2D map, float2 uv) {
	//get the sampled height map
	fixed4 height = pow(tex2D(map, uv), power);

	return lerp(tex1, tex2, sigmoid(sigmoidPower, height.r, control));
}

BurnState burnStateUV(float fireSize, float firePosition, float2 uv) {
	//get UV coordinates for Tree
	//Assume terrain will always be at 0,0
	BurnState output;
	output.burnState = sigmoid(5.0f, _BurnProgress - fireSize + firePosition, tex2D(_BurnMap, uv).r);
	output.fireLine = output.burnState - sigmoid(5.0f, _BurnProgress + firePosition, tex2D(_BurnMap, uv).r);
	return output;
}

// Get the burn edge and burn line based on parameters
BurnState currentBurnState(float fireSize, float firePosition, float3 worldPos) {
	//get UV coordinates for Tree
	//Assume terrain will always be at 0,0
	float2 relativePos = float2(worldPos.x / _RelativePosition.x, worldPos.z / _RelativePosition.z);
	return burnStateUV(fireSize, firePosition, relativePos);
}

//sample perlin noise
float perlinNoise(float2 uv) {
	return tex2D(_PerlinNoise, uv).r;
}

//Creates a blend effect based on an ink blotch style transition
float inkStainBlend(float2 uv, float control, float power) {
	return clamp(sigmoid(power, control, pow(perlinNoise(uv), 2.0)), 0, 1);
}

float inkStainBlend(float2 uv, float control) {
	return inkStainBlend(uv, control, 8.0);
}



//Creates a blend effect similar to burning paper
float paperBurnBlend(float2 uv, float control, float edgeSize) {
	return inkStainBlend(uv, control - edgeSize, 15) - inkStainBlend(uv, control, 15);
}