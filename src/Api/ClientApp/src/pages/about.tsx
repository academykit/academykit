import ReactMarkdown from "react-markdown";
import { Container } from "@mantine/core";

export const AboutPage = () => {
  const markDownContent = `
# About Us

## Why join Vurilo.com?

Here you can share your learning as well as learn from others. Vurilo supports both English and Nepali, so that is easily accessible worldwide as well as for the locals here.
The courses designed here are made in such a way that everyone will be able to comprehend.
In Vurilo, we do have the availability of a narrator, so if anyone is camera-shy the narrator will record the courses on their behalf.
Another reason Vurilo stands out in front of other learning platforms is because of affordability and accessibility of easy in-app payment gateway.

## What will Students get to learn?

We believe that learning never stops either you’re four or forty you can always learn.
Here Members who are passionate to learn can sharpen their knowledge base and learn from your comfort Zone.
Students will be provided with various options from which they can choose, according to their own convenience.
What matters the most for Vurilo is your passion and desire for learning, therefore we created this platform so that anyone can learn from their own comfort zone.

## How can Vurilo be beneficial to a Teacher?

Knowledge increases as you share it, hence if you share your knowledge here you will be doing a noble work of teaching as well as being able to work towards your passion.
By this you are giving back to society what you are skilled at and at the same time making an extra income.
Well networking and getting connected with the Members are yet another aspect the Teachers would be benefited with.

Our mission is to provide a platform where learning happens anytime, anywhere for anyone.

    `;

  return (
    <Container>
      <ReactMarkdown children={markDownContent} />
    </Container>
  );
};

export default AboutPage;
