uint2 textureSize;
Texture.GetDimensions(textureSize.x, textureSize.y);

bool inPoint = false;
int pointIndex = 0;
Result[id.y] = (segment)0;
segment currentSegment = (segment)0;
int start = 0;
for (uint i = 0; i < textureSize.x + 1; i++){
    float value = Texture.Load(int3(i,id.y,0)).r * (i<textureSize.x);
    bool newInPoint = value.r > 0;

    
    //Point start
    if (newInPoint && !inPoint){
        currentSegment = (segment)0;
        if (MaxSegmentsInRow==pointIndex){//last segment written            
            currentSegment.start_length = 0xffff << 16;
            Result[id.y + textureSize.y*(pointIndex-1)] = currentSegment;
            return;        
        }

        start = i;
        currentSegment.start_length = i << 16;
    }
    if (newInPoint){ 
        int x = i - start;
        currentSegment.start_length += 1;
        currentSegment.s += value;
        currentSegment.sx += x * value;
        currentSegment.sxx += x * x * value;      
    }
    //Result[id.y + textureSize.y*pointIndex] = currentSegment;
    //Point end
    if (!newInPoint && inPoint){
        Result[id.y + textureSize.y*pointIndex] = currentSegment;
        pointIndex++;        
    }
    inPoint = newInPoint;
}
Result[id.y + textureSize.y*pointIndex] = (segment)0;


