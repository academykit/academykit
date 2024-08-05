export type BrandingThemeType = [
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
];

const mixColors = (color1: string, color2: string, percent: number) => {
  const p = percent / 100;
  const R1 = parseInt(color1.substring(0, 2), 16);
  const G1 = parseInt(color1.substring(2, 4), 16);
  const B1 = parseInt(color1.substring(4, 6), 16);

  const R2 = parseInt(color2.substring(0, 2), 16);
  const G2 = parseInt(color2.substring(2, 4), 16);
  const B2 = parseInt(color2.substring(4, 6), 16);

  const mixedR = Math.round(R1 * (1 - p) + R2 * p);
  const mixedG = Math.round(G1 * (1 - p) + G2 * p);
  const mixedB = Math.round(B1 * (1 - p) + B2 * p);

  return `${mixedR.toString(16).padStart(2, "0")}${mixedG
    .toString(16)
    .padStart(2, "0")}${mixedB.toString(16).padStart(2, "0")}`;
};

const generateTints = (color: string, numTints: number) => {
  const tints = [];
  const baseColor = color.replace("#", "");

  for (let i = 1; i <= numTints; i++) {
    // can be adjusted
    const percent = i * 3;
    const tintColor = mixColors(baseColor, "000000", percent);
    tints.push(`#${tintColor}`);
  }

  return tints as BrandingThemeType;
};

export default generateTints;
