import { EditSectionProvider } from "@context/SectionProvider";
import EditSection from "./Components/EditSection";

const CourseLessons = () => {
  return (
    <EditSectionProvider>
      <EditSection />
    </EditSectionProvider>
  );
};

export default CourseLessons;
