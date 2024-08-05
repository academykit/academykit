import qs from 'query-string';

const queryStringGenerator = (data: any) => {
  const finalData: any = {};
  Object.keys(data).map((x) => {
    if (data[x]) {
      if (typeof data[x] === 'number') {
        if (data[x] > 0) {
          finalData[x] = data[x];
        }
      } else {
        finalData[x] = data[x];
      }
    }
  });
  return qs.stringify(finalData);
};

export default queryStringGenerator;
