export const getInitials = (data: string) => {
  const names = data?.split(" ");
  let initials = names && names[0].substring(0, 1).toUpperCase();

  if (names && names.length > 1) {
    initials += names[names.length - 1].substring(0, 1).toUpperCase();
  }
  return initials;
};
