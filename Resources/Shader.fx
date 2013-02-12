/////////////////
/// Variables ///
/////////////////

float4x4 projMat;			// Project Matrix.
float4x4 viewMat;			// View Matrix.
float4x4 worldMat;			// World Matrix.
float4x4 lightWVP;			// Light World View Projection Matrix.
float4 lightPos[3];			// Position of the lights.
float4 eyePos;				// Position of the camera.

texture ambientMap;
texture diffuseMap;	
texture specularMap;	
texture normalMap;

texture glowMap;
texture renderedScene;
texture shadowMapLight1;
texture shadowMapLight2;
texture shadowMapLight3;

float lightIntensity[3];
float glowStrength = 3.0f;
float maxDepth = 60.0f;

float4 lightAmbientColor[3];
float4 lightDiffuseColor[3];
float4 lightSpecularColor[3];

bool useDiffuseMap = false;
bool useSpecularMap = false;
bool useSpecularAlpha = false;
bool useNormalMap = false;
bool useAmbientMap = false;
int enableLight[3];
int enableShadows[3];

int shine = 100;
int minSamples = 5;
int maxSamples = 50;
float normalStrength = 1.0f;
float heightMapScale = 0.05f;
float shadowBias = 1.0f/250.0f;

const float2 offsets[12] = {
   -0.326212, -0.405805,
   -0.840144, -0.073580,
   -0.695914,  0.457137,
   -0.203345,  0.620716,
    0.962340, -0.194983,
    0.473434, -0.480026,
    0.519456,  0.767022,
    0.185461, -0.893124,
    0.507431,  0.064425,
    0.896420,  0.412458,
   -0.321940, -0.932615,
   -0.791559, -0.597705,
};

///////////////
/// Samplers ///
///////////////

sampler2D ambientSampler : TEXUNIT0 = sampler_state
{
	Texture = (ambientMap);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D glowSampler : TEXUNIT0 = sampler_state
{
	Texture = (glowMap);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D lightOneShadowSampler : TEXUNIT0 = sampler_state
{
	Texture = (shadowMapLight1);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D lightTwoShadowSampler : TEXUNIT0 = sampler_state
{
	Texture = (shadowMapLight2);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D lightThreeShadowSampler : TEXUNIT0 = sampler_state
{
	Texture = (shadowMapLight3);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D sceneSampler : TEXUNIT0 = sampler_state
{
	Texture = (renderedScene);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D diffuseSampler : TEXUNIT0 = sampler_state
{
	Texture = (diffuseMap);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D specularSampler : TEXUNIT1 = sampler_state
{
	Texture = (specularMap);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

sampler2D normalSampler : TEXUNIT2 = sampler_state
{
	Texture = (normalMap);
	MIPFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MINFILTER = ANISOTROPIC;
};

////////////////
/// Structures ///
////////////////

// Application to vertex structure
struct a2v
{
	float4 position : POSITION0;
	float3 normal : NORMAL;
	float2 tex0 : TEXCOORD0;
	float4 tangent : TANGENT;
};

// Vertex to pixel shader structure
struct v2p
{
	float4 position : POSITION0;
	float2 tex0 : TEXCOORD0;
	float3 lightVec : TEXCOORD1;
	float3 halfwayVec : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float4 tangent : TEXCOORD4;
	float4 shadowSamplingPos : TEXCOORD5;
	float4 realDistance : TEXCOORD6;
};

// Vertex to pixel shader for parallax mapping
struct v2parallax
{
	float4 position : POSITION0;
	float2 tex0 : TEXCOORD0;
	float3 lightVec : TEXCOORD1;
	float3 halfwayVec : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 eye : TEXCOORD4;
	float4 tangent : TEXCOORD5;
	float4 shadowSamplingPos : TEXCOORD6;
	float4 realDistance : TEXCOORD7;
};

struct PS_INPUT 
{ 
    float2 TexCoord : TEXCOORD0; 
}; 

struct SMapVertexToPixel
{
	float4 Position     : POSITION;    
	float3 Position2D    : TEXCOORD0;
};

//pixel shader to screen
struct p2f
{
     float4 color : COLOR0;
};

///////////////
/// Function ///
///////////////

// Normal Mapping

v2p vsNormalMap( in a2v input, uniform int currentLight )
{
	v2p output = (v2p)0;
	
	// Position in world space.
	float4 posWorld = mul(input.position, worldMat);
	float3 binormal = cross(input.tangent.xyz, input.normal);

	//Creating the Tangent, Binormal and Normal matrix
	float3x3 tanMat = transpose(float3x3(input.tangent.xyz, binormal, input.normal));
	
	// Calculating the light vector and eye vector.
	float3 lightVec = (lightPos[currentLight].xyz - input.position.xyz); 
	float3 eyeVec = (eyePos.xyz - input.position.xyz);
	
	// Calculate the light vector and halfway vector for tangent space.
	output.lightVec = normalize(mul(lightVec, tanMat));
	output.halfwayVec = normalize(mul((eyeVec + lightVec), tanMat));
	
	output.tex0 = input.tex0; // Set the texture coordinates
	output.position = mul(posWorld, mul(viewMat, projMat));
	output.normal = normalize(mul(input.normal, (float3x3)worldMat));
	output.tangent = normalize(mul(input.tangent, worldMat));
	
	return output;
}

p2f psNormalMap( in v2p input, uniform int currentLight )
{
	p2f output = (p2f)0;
		
   float3 color = float3(1.0f, 1.0f, 1.0f);//, 0.0f);		// Creates and sets the detault color
	float4 specColor = float4(0.7f, 0.7f, 0.7f, 0.0f);	// Creates and sets the default color
   float2 lightingInfo;											// Creates the specular and diffuse variables
	float3 normal = tex2D(normalSampler, input.tex0).xyz * 2.0f - 1.0f;
	   
	if(useDiffuseMap)
		color = tex2D(diffuseSampler, input.tex0);
	
	if(useSpecularMap)
		specColor = tex2D(specularSampler, input.tex0);

	normal = normalize(normal);	
	input.lightVec = normalize(input.lightVec);
	input.halfwayVec = normalize(input.halfwayVec);
	
	if(input.tangent.w < 0)
		normal.y *= -1.0f;
	
	normal.xy *= normalStrength;
	
	lightingInfo.x = 0.0f;
	lightingInfo.y = 0.0f;
	
	float distance = 0;
	
	lightingInfo.x = max(dot(normal, input.lightVec), 0);
			
	// Calculates the amount of specular on the pixel.
	if(useSpecularAlpha && useSpecularMap)
		lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
	else
		lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);

		
	if(lightingInfo.x < lightAmbientColor[currentLight].r)
		lightingInfo.x = lightAmbientColor[currentLight].rgb;
		
	//Adds the diffuse color and the specular color to obtain the final color.
	output.color.rgb = saturate((lightingInfo.x * color.rgb * lightDiffuseColor[currentLight].rgb) + 
										 (lightingInfo.y * specColor.rgb * lightSpecularColor[currentLight].rgb));
	output.color.rgb *= lightIntensity[currentLight];
		
	if(useAmbientMap)
		output.color.rgb *= tex2D(ambientSampler, input.tex0);
	
	if(!enableLight[currentLight])
		output.color = float4(0, 0, 0, 0);
	
	return output;
}

// Normal Mapping with Shadows

v2p vsNormalMapShadows( in a2v input, uniform int currentLight )
{
	v2p output = (v2p)0;
	
	// Position in world space.
	float4 posWorld = mul(input.position, worldMat);
	
	float3 binormal = cross(input.tangent.xyz, input.normal);

	//Creating the Tangent, Binormal and Normal matrix
	float3x3 tanMat = transpose(float3x3(input.tangent.xyz, binormal, input.normal));
	
	// Calculating the light vector and eye vector.
	float3 lightVec = (lightPos[currentLight].xyz - input.position.xyz); 
	float3 eyeVec = (eyePos.xyz - input.position.xyz);
	
	// Calculate the light vector and halfway vector for tangent space.
	output.lightVec = normalize(mul(lightVec, tanMat));
	output.halfwayVec = normalize(mul((eyeVec + lightVec), tanMat));
	
	output.shadowSamplingPos = mul(input.position, lightWVP);
	output.realDistance = output.shadowSamplingPos.z / maxDepth;
	
	output.tex0 = input.tex0; // Set the texture coordinates
	output.position = mul(posWorld, mul(viewMat, projMat));
	output.normal = normalize(mul(input.normal, (float3x3)worldMat));
	output.tangent = normalize(mul(input.tangent, worldMat));
	
	return output;
}

p2f psNormalMapShadows( in v2p input, uniform int currentLight )
{
	p2f output = (p2f)0;
		
   float3 color = float3(1.0f, 1.0f, 1.0f);//, 0.0f);		// Creates and sets the detault color
	float4 specColor = float4(0.7f, 0.7f, 0.7f, 0.0f);	// Creates and sets the default color
   float2 lightingInfo;											// Creates the specular and diffuse variables
	float3 normal = tex2D(normalSampler, input.tex0).xyz * 2.0f - 1.0f;
	float2 projectedTexCoords;
	
   projectedTexCoords.x = input.shadowSamplingPos.x/input.shadowSamplingPos.w/2.0f + 0.5f;
   projectedTexCoords.y = -input.shadowSamplingPos.y/input.shadowSamplingPos.w/2.0f + 0.5f;
	
	if(useDiffuseMap)
		color = tex2D(diffuseSampler, input.tex0);
	
	if(useSpecularMap)
		specColor = tex2D(specularSampler, input.tex0);

	normal = normalize(normal);	
	input.lightVec = normalize(input.lightVec);
	input.halfwayVec = normalize(input.halfwayVec);
	
	if(input.tangent.w < 0)
		normal.y *= -1.0f;
	
	normal.xy *= normalStrength;
	
	lightingInfo.x = 0.0f;
	lightingInfo.y = 0.0f;
	
	float distance = 0;
	
	if(currentLight == 0)
		distance = tex2D(lightOneShadowSampler, projectedTexCoords).x;
//	else if(currentLight == 1)
//		distance = tex2D(lightTwoShadowSampler, projectedTexCoords).x;
//	else if(currentLight == 2)
//		distance = tex2D(lightThreeShadowSampler, projectedTexCoords).x;

	if ((saturate(projectedTexCoords.x) == projectedTexCoords.x) && 
		 (saturate(projectedTexCoords.y) == projectedTexCoords.y) && 
		 enableShadows[currentLight] && currentLight == 0)
	{    
		if ((input.realDistance.x - shadowBias) <= distance)
		{
			lightingInfo.x = max(dot(normal, input.lightVec), 0);
			
			// Calculates the amount of specular on the pixel.
			if(useSpecularAlpha && useSpecularMap)
				lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
			else
				lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);
		}
	}
	else
	{
		lightingInfo.x = max(dot(normal, input.lightVec), 0);
			
		// Calculates the amount of specular on the pixel.
		if(useSpecularAlpha && useSpecularMap)
			lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
		else
			lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);
	}
	
	if(lightingInfo.x < lightAmbientColor[currentLight].r)
		lightingInfo.x = lightAmbientColor[currentLight].rgb;
		
	//Adds the diffuse color and the specular color to obtain the final color.
	output.color.rgb = saturate((lightingInfo.x * color.rgb * lightDiffuseColor[currentLight].rgb) + 
										 (lightingInfo.y * specColor.rgb * lightSpecularColor[currentLight].rgb));
	output.color.rgb *= lightIntensity[currentLight];
		
	if(useAmbientMap)
		output.color.rgb *= tex2D(ambientSampler, input.tex0);
	
	if(!enableLight[currentLight])
		output.color = float4(0, 0, 0, 0);
	
	return output;
}

// Per Pixel Lighting

v2p vsPerPixel( in a2v input, uniform int currentLight)
{
	v2p output = (v2p)0;
	
	// Position in world space.
	output.position = mul(input.position, worldMat);
	output.normal = normalize(mul(input.normal, (float3x3)worldMat));
	
	float3 eyeVec = normalize(eyePos.xyz - output.position.xyz);
	output.lightVec.xyz = normalize(lightPos[currentLight].xyz - output.position.xyz);
	output.halfwayVec = normalize(eyeVec + output.lightVec);

	output.tex0 = input.tex0;
	output.position = mul(output.position, mul(viewMat, projMat));
		
	return output;
}

p2f psPerPixel( in v2p input, uniform int currentLight)
{
	p2f output = (p2f)0;
	
   float4 color = float4(1.0f, 1.0f, 1.0f, 0.0f);		// Creates and sets the detault color
	float4 specColor = float4(0.7f, 0.7f, 0.7f, 0.0f);	// Creates and sets the default color
   float2 lightingInfo;											// Creates the specular and diffuse variables
   float3 normal = input.normal;								// Sets the normal to the incoming normal.
	   
	if(useDiffuseMap)
		color = tex2D(diffuseSampler, input.tex0);
	
	if(useSpecularMap)
		specColor = tex2D(specularSampler, input.tex0);

	normal = normalize(normal);
	input.lightVec = normalize(input.lightVec);
	input.halfwayVec = normalize(input.halfwayVec);
	
	lightingInfo.x = 0.0f;
	lightingInfo.y = 0.0f;
	
	float distance;
	
	lightingInfo.x = max(dot(normal, input.lightVec), 0);
	
	// Calculates the amount of specular on the pixel.
	if(useSpecularAlpha && useSpecularMap)
		lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
	else
		lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);
	
	if(lightingInfo.x < lightAmbientColor[currentLight].r)
		lightingInfo.x = lightAmbientColor[currentLight].rgb;
	
	//Adds the diffuse color and the specular color to obtain the final color.
	output.color.rgb = saturate((lightingInfo.x * color.rgb * lightDiffuseColor[currentLight].rgb) + 
										 (lightingInfo.y * specColor.rgb * lightSpecularColor[currentLight].rgb));
	output.color.rgb *= lightIntensity[currentLight];
		
	if(useAmbientMap)
		output.color.rgb *= tex2D(ambientSampler, input.tex0);
	
	if(!enableLight[currentLight])
		output.color = float4(0, 0, 0, 0);
	
	return output;
}

// Per Pixel Lighting with Shadows

v2p vsPerPixelShadow( in a2v input, uniform int currentLight)
{
	v2p output = (v2p)0;
	
	// Position in world space.
	output.position = mul(input.position, worldMat);
	
	output.normal = normalize(mul(input.normal, (float3x3)worldMat));
	
	float3 eyeVec = normalize(eyePos.xyz - output.position.xyz);
	output.lightVec.xyz = normalize(lightPos[currentLight].xyz - output.position.xyz);
	output.halfwayVec = normalize(eyeVec + output.lightVec);
	
	output.shadowSamplingPos = mul(input.position, lightWVP);
	output.realDistance = output.shadowSamplingPos.z / maxDepth;
	
	output.tex0 = input.tex0;
	output.position = mul(output.position, mul(viewMat, projMat));
		
	return output;
}

p2f psPerPixelShadow( in v2p input, uniform int currentLight)
{
	p2f output = (p2f)0;
	
   float4 color = float4(1.0f, 1.0f, 1.0f, 0.0f);		// Creates and sets the detault color
	float4 specColor = float4(0.7f, 0.7f, 0.7f, 0.0f);	// Creates and sets the default color
   float2 lightingInfo;											// Creates the specular and diffuse variables
   float3 normal = input.normal;								// Sets the normal to the incoming normal.
	float2 projectedTexCoords;
	
   projectedTexCoords.x = input.shadowSamplingPos.x/input.shadowSamplingPos.w/2.0f + 0.5f;
   projectedTexCoords.y = -input.shadowSamplingPos.y/input.shadowSamplingPos.w/2.0f + 0.5f;
   
	if(useDiffuseMap)
		color = tex2D(diffuseSampler, input.tex0);
	
	if(useSpecularMap)
		specColor = tex2D(specularSampler, input.tex0);

	normal = normalize(normal);
	input.lightVec = normalize(input.lightVec);
	input.halfwayVec = normalize(input.halfwayVec);
	
	lightingInfo.x = 0.0f;
	lightingInfo.y = 0.0f;
	
	float distance = 0;
	
	if(currentLight == 0)
		distance = tex2D(lightOneShadowSampler, projectedTexCoords).x;
//	else if(currentLight == 1)
//		distance = tex2D(lightTwoShadowSampler, projectedTexCoords).x;
//	else if(currentLight == 2)
//		distance = tex2D(lightThreeShadowSampler, projectedTexCoords).x;
	
	if ((saturate(projectedTexCoords.x) == projectedTexCoords.x) && 
		 (saturate(projectedTexCoords.y) == projectedTexCoords.y) && 
		 enableShadows[currentLight] && currentLight == 0)
	{    
		if ((input.realDistance.x - shadowBias) <= distance)
		{
			lightingInfo.x = max(dot(normal, input.lightVec), 0);
			
			// Calculates the amount of specular on the pixel.
			if(useSpecularAlpha && useSpecularMap)
				lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
			else
				lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);
		}
	}
	else
	{
		lightingInfo.x = max(dot(normal, input.lightVec), 0);
			
		// Calculates the amount of specular on the pixel.
		if(useSpecularAlpha && useSpecularMap)
			lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
		else
			lightingInfo.y = pow(max(dot(normal, input.halfwayVec), 0), shine);
	}
	
	if(lightingInfo.x < lightAmbientColor[currentLight].r)
		lightingInfo.x = lightAmbientColor[currentLight].rgb;
	
	//Adds the diffuse color and the specular color to obtain the final color.
	output.color.rgb = saturate((lightingInfo.x * color.rgb * lightDiffuseColor[currentLight].rgb) + 
										 (lightingInfo.y * specColor.rgb * lightSpecularColor[currentLight].rgb));
	output.color.rgb *= lightIntensity[currentLight];
		
	if(useAmbientMap)
		output.color.rgb *= tex2D(ambientSampler, input.tex0);
	
	if(!enableLight[currentLight])
		output.color = float4(0, 0, 0, 0);
	
	return output;
}

// Parallax Mapping

v2parallax vsParallaxMap( in a2v input, uniform int currentLight )
{
	v2parallax output = (v2parallax)0;
	
	// Position in world space.
	float4 posWorld = mul(input.position, worldMat);
	
	float3 binormal = cross(input.tangent.xyz, input.normal);
	
	//Creating the Tangent, Binormal and Normal matrix
	float3x3 tanMat = transpose(float3x3( input.tangent.xyz, binormal, input.normal ));
	
	// Calculating the light vector and eye vector.
	float3 lightVec = (lightPos[currentLight] - input.position.xyz); 
	float3 eyeVec = (eyePos - input.position.xyz);
	
	// Calculate the light vector and halfway vector for tangent space.
	output.lightVec.xyz = normalize(mul(lightVec, tanMat));
	output.halfwayVec = normalize(mul((eyeVec + lightVec), tanMat));
		
	output.eye = mul(eyeVec, tanMat);	
	output.tex0 = input.tex0; // Set the texture coordinates
	output.position = mul(posWorld, mul(viewMat, projMat));
	output.normal = normalize(mul(input.normal, (float3x3)worldMat));
	
	return output;
}

p2f psParallaxMap( in v2parallax input, uniform int currentLight )
{
	p2f output = (p2f)0;
	
   float4 color = float4(1.0f, 1.0f, 1.0f, 0.0f);		// Creates and sets the detault color
	float4 specColor = float4(0.7f, 0.7f, 0.7f, 0.0f);	// Creates and sets the default color
   float3 specular, diffuse;									// Creates the specular and diffuse variables
   float3 normal = input.normal;								// Sets the normal to the incoming normal.

	float parallaxLimit = length(input.eye.xy) / input.eye.z;
	parallaxLimit *= heightMapScale;
	
	float2 offset = normalize(-input.eye.xy);
	offset *= parallaxLimit;
	
	float3 eye = normalize(input.eye);
	
	int numSamples = (int)lerp(minSamples, maxSamples, dot(eye, normal));
	float stepSize = 1.0 / (float)numSamples;
	
	float2 dx, dy;
	dx = ddx(input.tex0);
	dy = ddy(input.tex0);
	
	float2 offsetStep = stepSize * offset;
	float2 currOffset = float2(0, 0);
	float2 lastOffset = float2(0, 0);
	float2 finalOffset = float2(0, 0);
	
	float4 currSample = float4(0, 0, 0, 0);
	float4 lastSample = float4(0, 0, 0, 0);
	
	float stepHeight = 1.0;
	int currSampleNum = 0;
	
	while(currSampleNum < numSamples)
	{
		currSample = tex2Dgrad(normalSampler, input.tex0 + currOffset, dx, dy);
				
		if(currSample.a > stepHeight)
		{
			float Ua = (lastSample.a - (stepHeight + stepSize)) / (stepSize + (currSample.a - lastSample.a));
			finalOffset = lastOffset + Ua * offsetStep;
			
			currSample = tex2Dgrad(normalSampler, input.tex0 + finalOffset, dx, dy);
			currSampleNum = numSamples;
		}
		else
		{
			stepHeight -= stepSize;
			lastOffset = currOffset;
			currOffset += offsetStep;
			lastSample = currSample;
		}
		
		currSampleNum++;
	}
	
	if(useDiffuseMap)
		color = tex2D(diffuseSampler, input.tex0 + currOffset);
	
	if(useSpecularMap)
		specColor = tex2D(specularSampler, input.tex0 + currOffset);
	
	// Sets the normal to the calue obtained from the texture.
	normal = tex2D(normalSampler, input.tex0 + currOffset);
	normal = (normal - 0.5f) * 2.0f; 
	
	if(input.tex0.x < 0.0 || input.tex0.x > 1.0 || input.tex0.y < 0.0 || input.tex0.y > 1.0)
		normal.g *= -1.0f;
	
	normal.xy *= normalStrength;

	normal = normalize(normal);
	input.lightVec = normalize(input.lightVec);
	input.halfwayVec = normalize(input.halfwayVec);
	
	// Calculates the brightness of the pixel
	diffuse = max(dot(normal, input.lightVec), 0);
	
	if(diffuse.r < lightAmbientColor[currentLight].r)
		diffuse = lightAmbientColor[currentLight].rgb;
	
	// Calculates the amount of specular on the pixel.
	if(useSpecularAlpha && useSpecularMap)
		specular = pow(max(dot(normal, input.halfwayVec), 0), lerp(150, 0, specColor.a));
	else
		specular = pow(max(dot(normal, input.halfwayVec), 0), shine);
	
	//Adds the diffuse color and the specular color to obtain the final color.
	output.color.rgb = saturate((diffuse * color.rgb * lightDiffuseColor[currentLight].rgb) 
										 + (specular * specColor.rgb * lightSpecularColor[currentLight].rgb));
	output.color.rgb *= lightIntensity[currentLight];
	
	
	if(useAmbientMap)
		output.color.rgb *= tex2D(ambientSampler, input.tex0 + currOffset);
	
	if(!enableLight[currentLight])
		output.color = float4(0, 0, 0, 0);		
	
	return output;
}

// Bloom

float4 bloomEffect(PS_INPUT Input) : COLOR0 
{ 
	float4 scene = tex2D(sceneSampler, Input.TexCoord);
	float4 sum = tex2D(glowSampler, Input.TexCoord);
	scene += sum;

	//accumulate the color values from 12 neighboring pixels
	for(int i = 0; i < 12; i++)
	{
		sum += tex2D(glowSampler, Input.TexCoord + 0.01f * offsets[i]);
	}

	//average the sum
	sum /= 13;

	return scene + sum * glowStrength;
}

// Shadow Mapping Depth

SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
{
    SMapVertexToPixel Output = (SMapVertexToPixel)0;
    
    Output.Position = mul(inPos, lightWVP);
    Output.Position2D = Output.Position;
    
    return Output;    
}

float4 ShadowMapPixelShader(SMapVertexToPixel PSIn) : COLOR0
{
	return PSIn.Position2D.z/maxDepth;
}

/////////////////
/// Techniques ///
/////////////////
technique NormalMap
{
	pass p0
	{
		vertexshader = compile vs_2_0 vsNormalMap(0);
		pixelshader = compile ps_2_0 psNormalMap(0);
	}
	
	pass p1
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsNormalMap(1);
		pixelshader = compile ps_2_0 psNormalMap(1);
	}
	
	pass p2
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsNormalMap(2);
		pixelshader = compile ps_2_0 psNormalMap(2);
	}
}

technique NormalMapShadows
{
	pass p0
	{
		vertexshader = compile vs_2_0 vsNormalMapShadows(0);
		pixelshader = compile ps_2_b psNormalMapShadows(0);
	}
	
	pass p1
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsNormalMapShadows(1);
		pixelshader = compile ps_2_b psNormalMapShadows(1);
	}
	
	pass p2
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsNormalMapShadows(2);
		pixelshader = compile ps_2_b psNormalMapShadows(2);
	}
}

technique PerPixel
{
	pass p0
	{
		vertexshader = compile vs_2_0 vsPerPixel(0);
		pixelshader = compile ps_2_0 psPerPixel(0);
	}

	pass p1
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsPerPixel(1);
		pixelshader = compile ps_2_0 psPerPixel(1);
	}
	
	pass p2
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsPerPixel(2);
		pixelshader = compile ps_2_0 psPerPixel(2);
	}
}

technique PerPixelShadows
{
	pass p0
	{
		vertexshader = compile vs_2_0 vsPerPixelShadow(0);
		pixelshader = compile ps_2_0 psPerPixelShadow(0);
	}

	pass p1
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsPerPixelShadow(1);
		pixelshader = compile ps_2_0 psPerPixelShadow(1);
	}
	
	pass p2
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_2_0 vsPerPixelShadow(2);
		pixelshader = compile ps_2_0 psPerPixelShadow(2);
	}
}

technique ParallaxMap
{
	pass p0
	{
		vertexshader = compile vs_3_0 vsParallaxMap(0);
		pixelshader = compile ps_3_0 psParallaxMap(0);
	}
	
	pass p1
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_3_0 vsParallaxMap(1);
		pixelshader = compile ps_3_0 psParallaxMap(1);
	}
	
	pass p2
	{
		SRCBLEND = ONE;
		DESTBLEND = ONE;
		ALPHABLENDENABLE = true;
		
		vertexshader = compile vs_3_0 vsParallaxMap(2);
		pixelshader = compile ps_3_0 psParallaxMap(2);
	}
}

technique Bloom
{ 
	pass p0
	{ 
		ZEnable = true; 
		ZWriteEnable = true; 
		CullMode = none; 
		
		PixelShader = compile ps_2_0 bloomEffect(); 
	} 
}

technique ShadowMap
{
	pass p0
	{		
		VertexShader = compile vs_2_0 ShadowMapVertexShader();
		PixelShader = compile ps_2_0 ShadowMapPixelShader();
	}
}