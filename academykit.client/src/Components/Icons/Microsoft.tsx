const Microsoft = ({ ...props }) => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      xmlnsXlink="http://www.w3.org/1999/xlink"
      viewBox="0 0 48 48"
      width="50px"
      height="50px"
      {...props}
    >
      <rect
        x="6"
        y="6"
        transform="matrix(-1 -4.502011e-11 4.502011e-11 -1 28 28)"
        style={{ fill: "#FF5722" }}
        width="16"
        height="16"
      />
      <rect
        x="26"
        y="6"
        transform="matrix(-1 -4.502011e-11 4.502011e-11 -1 68 28)"
        style={{ fill: "#4CAF50" }}
        width="16"
        height="16"
      />
      <rect
        x="26"
        y="26"
        transform="matrix(-1 -4.502011e-11 4.502011e-11 -1 68 68)"
        style={{ fill: "#FFC107" }}
        width="16"
        height="16"
      />
      <rect
        x="6"
        y="26"
        transform="matrix(-1 -4.502011e-11 4.502011e-11 -1 28 68)"
        style={{ fill: "#03A9F4" }}
        width="16"
        height="16"
      />
    </svg>
  );
};

export default Microsoft;
