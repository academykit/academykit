export function displayTime(minutes: number) {
  if (minutes >= 60) {
    const hours = Math.floor(minutes / 60);
    return `${hours} hr`;
  } else {
    return `${minutes} min`;
  }
}
