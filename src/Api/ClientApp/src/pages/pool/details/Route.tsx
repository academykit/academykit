import { Route, Routes } from "react-router-dom";
import MCQDetails from ".";
import MCQPoolNav from "../Components/MCQPoolNav";
import Create from "./Create";
import EditQuestion from "./Edit";
import MCQQuestions from "./questions";
import MCQTeachers from "./teachers";

const MCQPoolRoute = () => {
  return (
    <Routes>
      <Route element={<MCQPoolNav />}>
        <Route path="/" element={<MCQDetails />} />
        <Route path="/questions" element={<MCQQuestions />} />
        <Route path="/authors" element={<MCQTeachers />} />
        <Route path="questions/create" element={<Create />} />
        <Route path="questions/edit/:slug" element={<EditQuestion />} />
      </Route>
    </Routes>
  );
};

export default MCQPoolRoute;
