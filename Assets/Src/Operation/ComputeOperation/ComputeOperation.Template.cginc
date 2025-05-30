#pragma kernel Main
@GetParameters

@GetBufferElementTypeDeclaration

RWStructuredBuffer<@GetBufferElementTypeName> Result;

[numthreads(8, 8, 1)]
void Main(uint3 id : SV_DispatchThreadID, uint3 groupID : SV_GroupID, uint3 groupThreadID : SV_GroupThreadID)
{
    @GetMainKernelBody


    //InterlockedMax(Result[0], value.r); // Atomic operation to find max

    /*// Flatten groupThreadID to index
    uint index = groupThreadID.y * 8 + groupThreadID.x;
    sharedMax[index] = value;
    GroupMemoryBarrierWithGroupSync();

    // Parallel reduction (in-place, assumes 64 threads)
    for (uint stride = 32; stride > 0; stride >>= 1)
    {
        if (index < stride)
            sharedMax[index] = max(sharedMax[index], sharedMax[index + stride]);

        GroupMemoryBarrierWithGroupSync();
    }

    // Write result from thread 0 of each group
    if (index == 0)
    {
        Result[groupID.y * DispatchSize.x + groupID.x] = sharedMax[0];
    }*/
}