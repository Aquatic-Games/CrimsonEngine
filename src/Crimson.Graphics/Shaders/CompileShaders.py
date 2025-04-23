import subprocess
import os
from pathlib import Path

def compile_shader(path: Path, stage: str, entry_point: str):
    file_name = path.stem
    
    profile = None
    
    match stage:
        case "vertex":
            profile = "vs_6_0"
            file_name += "_v"
        case "pixel":
            profile = "ps_6_0"
            file_name += "_p"
        case _:
            raise Exception(f"Unknown shader stage '{stage}'.")
    
    file_name += ".spv"
    
    out_path = path.parent.joinpath(file_name)
    
    print(f"{path.resolve()} -> {out_path.resolve()}")
    
    # replace forward slashes with backslashes on linux because fxc is fxc and uses forward slashes because thanks microsoft
    subprocess.run(['dxc', '-spirv', '-T', profile, '-E', entry_point, '-Fo', str(out_path), str(path)]).check_returncode()
    
if __name__ == "__main__":
    working_dir = Path('.')
    
    hlsl_files = working_dir.glob("**/*.hlsl")
    
    for file in hlsl_files:
        path = file.resolve()

        vertex_entpt = None
        pixel_entpt = None
        
        with open(path, 'r') as f:
            for l in f.readlines():
                line = l.strip()
                
                if line == "":
                    continue
                
                if line.startswith("#pragma"):
                    split_line = line.split(' ')
                    
                    if len(split_line) < 2:
                        continue
                        
                    match split_line[1]:
                        case "vertex":
                            vertex_entpt = split_line[2]
                        case "pixel":
                            pixel_entpt = split_line[2]
                else:
                    break
        
        if vertex_entpt is not None:
            compile_shader(path, "vertex", vertex_entpt)
        if pixel_entpt is not None:
            compile_shader(path, "pixel", pixel_entpt)
        