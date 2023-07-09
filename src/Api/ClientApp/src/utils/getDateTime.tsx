import moment from "moment";

const getDateTime = (startDate: Date, startTime: string) => {
  const date = new Date(startDate).toLocaleDateString();
  const a = moment(`${date} ${startTime}`,"DD/MM/YYYY HH:mm")
  return {
    time:a.toDate().toLocaleTimeString(),
    date,
    utcDateTime: a.toDate().toISOString(),
    localDateTime: a.toDate(),
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
