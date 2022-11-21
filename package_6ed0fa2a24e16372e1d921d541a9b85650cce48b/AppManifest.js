var UnoAppManifest = {

    splashScreenImage: "Assets/CodeHashifier.svg",
    splashScreenColor: "transparent",
    displayName: "CodeHashifier Online"

}

var link = document.querySelector("link[rel~='icon']");
if (!link) {
    link = document.createElement('link');
    link.rel = 'icon';
    document.getElementsByTagName('head')[0].appendChild(link);
}
link.href = config.uno_app_base + '/Assets/CodeHashifier.ico';