VSO vert(VSI input) {
    VSO result;

    result.position = input.position;
	result.position = UnityObjectToClipPos(result.position);  

    result.uv = input.uv;
    return result;
}