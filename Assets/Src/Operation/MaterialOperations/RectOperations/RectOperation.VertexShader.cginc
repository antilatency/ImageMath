VSO vert(VSI input) {
    VSO result;

    result.position = input.position;
    result.position.xy *= Size;
	result.position.xy += Position;
	result.position = UnityObjectToClipPos(result.position);  

    result.uv = input.uv;
    return result;
}