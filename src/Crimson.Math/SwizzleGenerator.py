def permutate(out_list: list[str], input: str, current: str, size: int):
    if size <= 0:
        return
    
    for c in input:
        new = current + c
        out_list.append(new)
        permutate(out_list, input, new, size - 1)
        

if __name__ == "__main__":
    letters = input("Enter letters you want to swizzle (e.g. ABC): ")
    count = int(input("Max permutation length? "))
    
    result = []
    permutate(result, letters, "", count)
    
    result.sort(key=lambda x: len(x))
    
    for text in result:
        if len(text) == 1:
            continue
        
        vector = f"Vector{len(text)}T<T>"
        comps = ", ".join(text)
        
        print(f"public {vector} {text} => new {vector}({comps});")
        print()