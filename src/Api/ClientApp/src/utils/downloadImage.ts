function downloadImage(src: string, name: string) {
    //@ts-ignore
    const img = document.createElement('img');
    img.crossOrigin = 'anonymous';  
    img.src = src;
    img.onload = () => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');
      canvas.width = img.width;
      canvas.height = img.height;
        //@ts-ignore
  
      ctx.drawImage(img, 0, 0);
      const a = document.createElement('a');
      a.download = name+` Certificate.png`;
      a.href = canvas.toDataURL('image/png');
      a.click();
    };
  }

  export default downloadImage