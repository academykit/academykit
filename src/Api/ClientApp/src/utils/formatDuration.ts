// eslint-disable-next-line no-self-compare
const isNaN = Number.isNaN || ((value) => value !== value);

const formatDuration = (duration: number, isShort?: boolean) => {
  // handle invalid times
  if (isNaN(duration) || duration === Infinity) {
    return "-";
  }

  let s = Math.floor(duration % 60);

  let m = Math.floor((duration / 60) % 60);

  let h = Math.floor(duration / 3600);
  // Check if we need to show hours
  const formattedHours = h > 0 ? `${h} ${isShort ? "h" : "hour"} ` : "";

  // If hours are showing, we may need to add a leading zero.
  // Always show at least one digit of minutes.
  const formattedMinutes = `${
    m > 0 ? `${m} ${isShort ? "m" : "minute"} ` : ""
  }`;

  // Check if leading zero is need for seconds
  const formattedSecs = s > 0 ? `${s} ${isShort ? "s" : "second"}` : "";

  return `${formattedHours}${formattedMinutes}${formattedSecs}`;
};

export default formatDuration;