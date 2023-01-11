const getDateTime = (startDate: string, startTime: string) => {
  const time = new Date(startTime).toLocaleTimeString();
  const date = new Date(startDate).toLocaleDateString();
  const utcDateTime = new Date(date + " " + time).toISOString();
  const localDateTime = new Date(date + " " + time);

  return {
    time,
    date,
    utcDateTime,
    localDateTime,
  };
};

const getDateAndTime = (dates?: string) => {
  if (dates) {
    const date = new Date(dates).toLocaleDateString();
    const time = new Date(dates).toLocaleTimeString();

    return {
      time,
      date,
    };
  } else {
    return { time: "", date: "" };
  }
};

export { getDateTime, getDateAndTime };
