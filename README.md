# AI-UI

AI-UI is an easy to use interface for Stable Diffusion built with C#/WPF and  Automatic1111's API, that visualizes your generation history in a tree format.


## Requirements

This app requires a version of Automatic1111 already set up (with a model installed). Additionally the WebUI batch file must have the `--api` argument. Automatic1111 requires Python and recommends version 3.10.6. Application settings (such as the selected model) must be set up from the Automatic1111 WebUI. AI-UI uses the .Net 6.0 Runtime, so that will also need to be installed.


## Getting Started
- First start the Automatic 1111 WebUI .bat with the `api` command and make sure it starts correctly.
- Then Build & Run AI-UI from Visual Studio. (After first build the App can be run without VS)
- In the Start Up Window you can create a new tree or load an existing one.
- In the left pane you can enter prompt information and enter other settings. Entering a prompt in the left box and the right box will result in these prompts being merged together for the image generation request.
- As images are generated a tree will be created on the right side of the window. This tree shows your history. Images with the same prompt will be collected in the same node. Double-Clicking on a node will open an image viewer for that collection of images. Left-clicking on a node will enter that node's prompt information in the prompt inputs on the left. Right-clicking a node will enter it's information in the right inputs. This can be used to merge existing prompts.
- Created images and trees can be found in the `output` directory.

## References and Downloads

- Visual Studio 2022: https://visualstudio.microsoft.com/
- Automatic 1111 WebUI: https://github.com/AUTOMATIC1111/stable-diffusion-webui
- Net 6.0 Download: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- Python 3.10.6: https://www.python.org/downloads/release/python-3106/
- Stable Diffusion Models can be found here: https://huggingface.co/models