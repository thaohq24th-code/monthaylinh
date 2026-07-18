using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới Staff (nhân viên).
    /// </summary>
    public class StaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        /// <summary>Lấy toàn bộ nhân viên.</summary>
        public async Task<List<Staff>> GetAllStaffsAsync()
        {
            return await _staffRepository.GetAllAsync();
        }

        /// <summary>Lấy nhân viên theo Id.</summary>
        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            return await _staffRepository.GetByIdAsync(id);
        }

        /// <summary>Thêm nhân viên mới.</summary>
        public async Task<(bool Success, string Message)> CreateStaffAsync(Staff staff)
        {
            if (string.IsNullOrWhiteSpace(staff.Name))
            {
                return (false, "Tên nhân viên không được để trống");
            }

            if (staff.Salary < 0)
            {
                return (false, "Lương không được là số âm");
            }

            await _staffRepository.AddAsync(staff);
            return (true, "Đã thêm nhân viên mới thành công");
        }

        /// <summary>Cập nhật thông tin nhân viên.</summary>
        public async Task<(bool Success, string Message)> UpdateStaffAsync(Staff staff)
        {
            var existing = await _staffRepository.GetByIdAsync(staff.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy nhân viên cần cập nhật");
            }

            existing.Name = staff.Name;
            existing.Phone = staff.Phone;
            existing.Shift = staff.Shift;
            existing.Position = staff.Position;
            existing.Salary = staff.Salary;

            await _staffRepository.UpdateAsync(existing);
            return (true, "Đã cập nhật thông tin nhân viên");
        }

        /// <summary>Xóa nhân viên theo Id.</summary>
        public async Task<(bool Success, string Message)> DeleteStaffAsync(int id)
        {
            var deleted = await _staffRepository.DeleteAsync(id);
            return deleted
                ? (true, "Đã xóa nhân viên thành công")
                : (false, "Không tìm thấy nhân viên cần xóa");
        }
    }
}
