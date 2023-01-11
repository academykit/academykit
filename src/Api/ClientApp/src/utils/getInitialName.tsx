export var getInitials = (data: string) => {
  var names = data?.split(" "),
    initials = names && names[0].substring(0, 1).toUpperCase();

  if (names && names.length > 1) {
    initials += names[names.length - 1].substring(0, 1).toUpperCase();
  }
  return initials;
};
